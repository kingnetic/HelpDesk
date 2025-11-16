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
        private readonly IUserSessionService _sessions;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;

        public AuthService(
            UserManager<User> userManager,
            IUserSessionService sessions,
            IConfiguration config,
            IHttpContextAccessor http)
        {
            _userManager = userManager;
            _sessions = sessions;
            _config = config;
            _http = http;
        }

        private HttpContext? Http => _http.HttpContext;

        public async Task RegisterAsync(string email, string fullName, string password)
        {
            var user = new User
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Customer");
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                throw new Exception("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var (token, jti, expiresAt) = GenerateJwt(user, roles);

            var ip = Http?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ua = Http?.Request.Headers["User-Agent"].ToString() ?? "unknown";

            await _sessions.CreateSessionAsync(user.Id, jti, expiresAt, ip, ua);

            return token;
        }

        public async Task LogoutAsync(int userId, Guid jwtId, string? ip, string? userAgent)
        {
            await _sessions.CloseSessionByJwtIdAsync(jwtId, "Logout", ip, userAgent);
        }

        public async Task<UserDto> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                       ?? throw new Exception("User not found");

            return new UserDto(user.Id, user.Email!, user.FullName);
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
    }
}
