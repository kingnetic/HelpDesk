namespace HelpDesk.Application.Interfaces
{
    public interface IUserSessionService
    {
        Task<int> CreateSessionAsync(int userId, Guid jwtId, DateTime expiresAt, string? ip, string? userAgent);
        Task CloseSessionAsync(int sessionId, string? reason, string? ip, string? userAgent);
        Task CloseSessionByJwtIdAsync(Guid jwtId, string? reason, string? ip, string? userAgent);
        Task<int?> GetActiveSessionIdForUserAsync(int userId);
    }
}
