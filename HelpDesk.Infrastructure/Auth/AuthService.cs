using System;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.Auth;
using HelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelpDesk.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserSessionService _sessions;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;
        private readonly IEmailSender _emailSender;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserSessionService sessions,
            IConfiguration config,
            IHttpContextAccessor http,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sessions = sessions;
            _config = config;
            _http = http;
            _emailSender = emailSender;
        }

        private HttpContext? Http => _http.HttpContext;

        public async Task RegisterAsync(string email, string fullName, string password)
        {
            // Verificar si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new HelpDesk.Domain.Exceptions.ConflictException($"El email '{email}' ya está registrado en el sistema.");
            }

            var user = new User
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();

                // Si es un error de validación de contraseña
                if (errors.Any(e => e.Contains("password", StringComparison.OrdinalIgnoreCase)))
                {
                    var validationEx = new HelpDesk.Domain.Exceptions.ValidationException("La contraseña no cumple con los requisitos.");
                    validationEx.Errors["Password"] = errors.ToArray();
                    throw validationEx;
                }

                throw new HelpDesk.Domain.Exceptions.DomainException(string.Join(". ", errors));
            }

            await _userManager.AddToRoleAsync(user, HelpDesk.Domain.Constants.Roles.Customer);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var baseUrl = _config["AppSettings:BaseUrl"] ?? "http://localhost:5021";
            var confirmationLink = $"{baseUrl}/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendEmailConfirmationAsync(email, fullName, confirmationLink);
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new HelpDesk.Domain.Exceptions.UnauthorizedException("Credenciales inválidas.");

            if (!user.EmailConfirmed)
                throw new HelpDesk.Domain.Exceptions.UnauthorizedException("Email no confirmado. Por favor revisa tu correo para confirmar tu cuenta.");

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var minutesRemaining = lockoutEnd.HasValue
                    ? (int)(lockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes
                    : 0;
                throw new HelpDesk.Domain.Exceptions.ForbiddenException($"Cuenta bloqueada. Intenta de nuevo en {minutesRemaining} minutos.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new HelpDesk.Domain.Exceptions.ForbiddenException("Cuenta bloqueada por múltiples intentos fallidos. Intenta de nuevo en 10 minutos.");
                }

                var accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                var attemptsRemaining = 5 - accessFailedCount;

                throw new HelpDesk.Domain.Exceptions.UnauthorizedException($"Credenciales inválidas. {attemptsRemaining} intentos restantes antes del bloqueo.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var (token, jti, expiresAt) = GenerateJwt(user, roles);
            var refreshToken = GenerateRefreshToken();

            var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ua = _http.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown";

            await _sessions.CreateSessionAsync(user.Id, jti, expiresAt, ip, ua, refreshToken, DateTime.UtcNow.AddDays(7));

            return new AuthResult(token, refreshToken);
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new HelpDesk.Domain.Exceptions.NotFoundException("Usuario no encontrado.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                var validationEx = new HelpDesk.Domain.Exceptions.ValidationException("Error al cambiar la contraseña.");
                validationEx.Errors["Password"] = errors.ToArray();
                throw validationEx;
            }

            await _emailSender.SendPasswordChangedNotificationAsync(user.Email!, user.FullName ?? string.Empty);
        }

        public async Task<bool> ConfirmEmailAsync(int userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task LogoutAsync(int userId, Guid jwtId, string? ip, string? userAgent)
        {
            await _sessions.CloseSessionByJwtIdAsync(jwtId, "Logout", ip, userAgent);
        }

        public async Task<UserDto> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new HelpDesk.Domain.Exceptions.NotFoundException("Usuario no encontrado.");

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto(user.Id, user.Email!, user.FullName ?? string.Empty, roles.ToList());
        }

        private (string token, Guid jti, DateTime expiresAt) GenerateJwt(User user, IList<string> roles)
        {
            var jwt = _config.GetSection("JwtSettings");

            var secret = jwt["Key"] ?? throw new Exception("JwtSettings:Key missing");
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var expirationMinutes = int.Parse(jwt["ExpirationMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jti = Guid.NewGuid();
            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), jti, expiresAt);
        }

        public async Task<AuthResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            if (principal == null) throw new SecurityTokenException("Invalid token");

            var jtiString = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (!Guid.TryParse(jtiString, out var jti)) throw new SecurityTokenException("Invalid JTI");

            var session = await _sessions.GetSessionByRefreshTokenAsync(refreshToken);
            if (session == null) throw new SecurityTokenException("Invalid refresh token");

            if (session.JwtId != jti) throw new SecurityTokenException("Token mismatch");
            if (session.RefreshTokenExpiresAt < DateTime.UtcNow) throw new SecurityTokenException("Refresh token expired");
            if (!session.IsActiveSession) throw new SecurityTokenException("Session is inactive");

            var userId = int.Parse(principal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _userManager.GetRolesAsync(user!);

            var (newToken, newJti, newExpiresAt) = GenerateJwt(user!, roles);
            var newRefreshToken = GenerateRefreshToken();

            await _sessions.CloseSessionAsync(session.Id, "Token Refreshed", _http.HttpContext?.Connection.RemoteIpAddress?.ToString(), _http.HttpContext?.Request.Headers["User-Agent"].ToString());

            await _sessions.CreateSessionAsync(
                user!.Id,
                newJti,
                newExpiresAt,
                _http.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                _http.HttpContext?.Request.Headers["User-Agent"].ToString(),
                newRefreshToken,
                DateTime.UtcNow.AddDays(7));

            return new AuthResult(newToken, newRefreshToken);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
