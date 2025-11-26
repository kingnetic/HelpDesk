using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(string email, string fullName, string password);
        Task<HelpDesk.Application.DTOs.AuthResult> LoginAsync(string email, string password);
        Task LogoutAsync(int userId, Guid jwtId, string? ip, string? userAgent);
        Task<UserDto> GetCurrentUserAsync(string email);
        Task<HelpDesk.Application.DTOs.AuthResult> RefreshTokenAsync(string token, string refreshToken);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ConfirmEmailAsync(int userId, string token);
    }
}
