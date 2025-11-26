using HelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema y envía email de confirmación.
        /// </summary>
        /// <param name="model">Datos de registro (email, nombre, password).</param>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            await _auth.RegisterAsync(model.Email, model.FullName, model.Password);
            return Ok(new { message = "User created successfully. Please check your email to confirm your account." });
        }

        /// <summary>
        /// Inicia sesión y obtiene un token JWT.
        /// Rate limited: 5 requests por minuto por IP.
        /// </summary>
        /// <param name="model">Credenciales de acceso.</param>
        /// <returns>Token JWT.</returns>
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var result = await _auth.LoginAsync(model.Email, model.Password);
            return Ok(result);
        }

        /// <summary>
        /// Cierra la sesión actual invalidando el token.
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var jtiString = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (!Guid.TryParse(jtiString, out var jti))
                return BadRequest("Invalid token JTI");

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ua = Request.Headers["User-Agent"].ToString();

            await _auth.LogoutAsync(userId, jti, ip, ua);

            return Ok("Logout registrado");
        }

        /// <summary>
        /// Obtiene la información del usuario autenticado actual.
        /// </summary>
        /// <returns>Datos del usuario.</returns>
        [Authorize]
        [HttpGet("user-info")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var dto = await _auth.GetCurrentUserAsync(email);
            return Ok(dto);
        }

        /// <summary>
        /// Cambia la contraseña del usuario autenticado y envía notificación por email.
        /// </summary>
        /// <param name="request">Contraseña actual y nueva contraseña.</param>
        /// <returns>Confirmación del cambio.</returns>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(HelpDesk.Application.DTOs.ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest(new { message = "New password and confirmation do not match" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _auth.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

            return Ok(new { message = "Password changed successfully. A confirmation email has been sent." });
        }

        /// <summary>
        /// Confirma el email del usuario mediante el token enviado por correo.
        /// Redirige a una página HTML amigable.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="token">Token de confirmación.</param>
        /// <returns>Redirige a página HTML de confirmación.</returns>
        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] int userId, [FromQuery] string token)
        {
            var result = await _auth.ConfirmEmailAsync(userId, token);

            if (result)
                return Redirect("/email-confirmed.html?success=true");

            return Redirect("/email-confirmed.html?success=false");
        }

        public record RegisterRequest(string Email, string FullName, string Password);
        public record LoginRequest(string Email, string Password);
    }
}
