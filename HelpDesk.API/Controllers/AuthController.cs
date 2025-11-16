using HelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            await _auth.RegisterAsync(model.Email, model.FullName, model.Password);
            return Ok("User created");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var token = await _auth.LoginAsync(model.Email, model.Password);
            return Ok(new { token });
        }

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

        [Authorize]
        [HttpGet("user-info")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var dto = await _auth.GetCurrentUserAsync(email);
            return Ok(dto);
        }

        public record RegisterRequest(string Email, string FullName, string Password);
        public record LoginRequest(string Email, string Password);
    }
}
