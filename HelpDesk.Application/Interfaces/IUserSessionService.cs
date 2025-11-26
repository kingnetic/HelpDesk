namespace HelpDesk.Application.Interfaces
{
    public interface IUserSessionService
    {
        Task<int> CreateSessionAsync(int userId, Guid jwtId, DateTime expiresAt, string? ip, string? userAgent, string? refreshToken = null, DateTime? refreshTokenExpiresAt = null);
        Task CloseSessionAsync(int sessionId, string? reason, string? ip, string? userAgent);
        Task CloseSessionByJwtIdAsync(Guid jwtId, string? reason, string? ip, string? userAgent);
        Task<int?> GetActiveSessionIdForUserAsync(int userId);
        Task<HelpDesk.Domain.Entities.Auth.UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);
    }
}
