namespace HelpDesk.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> GetEmailByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId);
        Task<List<HelpDesk.Application.DTOs.UserDto>> GetAllUsersAsync(CancellationToken ct);
        Task<int> CreateUserAsync(string username, string email, string password, string fullName, List<string> roles);
        Task<bool> UpdateUserAsync(int id, string email, string fullName, List<string> roles);
        Task<bool> DeleteUserAsync(int id);
    }
}
