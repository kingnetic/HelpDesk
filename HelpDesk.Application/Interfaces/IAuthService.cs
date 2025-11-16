using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(string email, string fullName, string password);
        Task<string> LoginAsync(string email, string password);
        Task LogoutAsync(int userId, Guid jwtId, string? ip, string? userAgent);
        Task<UserDto> GetCurrentUserAsync(string email);
    }
}
