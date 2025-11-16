namespace HelpDesk.Application.Interfaces
{
    public interface IUserService
    {
        Task<string> GetEmailByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId);
    }
}
