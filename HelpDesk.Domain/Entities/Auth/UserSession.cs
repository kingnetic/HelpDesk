
using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities.Auth;

public class UserSession : BaseEntity
{
    public int UserId { get; private set; }
    public Guid JwtId { get; private set; }
    public DateTime TokenExpiresAt { get; private set; }
    public DateTime LoginAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LogoutAt { get; private set; }
    public string? LoginIpAddress { get; private set; }
    public string? LoginUserAgent { get; private set; }
    public string? LogoutIpAddress { get; private set; }
    public string? LogoutUserAgent { get; private set; }
    public string? LogoutReason { get; private set; }
    public bool IsActiveSession { get; private set; } = true;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    private UserSession() { }

    public UserSession(int userId, Guid jwtId, DateTime expiresAt, string? ip, string? userAgent)
    {
        UserId = userId;
        JwtId = jwtId;
        TokenExpiresAt = expiresAt;
        LoginIpAddress = ip;
        LoginUserAgent = userAgent;
    }

    public void SetRefreshToken(string token, DateTime expiresAt)
    {
        RefreshToken = token;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void Close(string? reason, string? ip, string? userAgent)
    {
        LogoutAt = DateTime.UtcNow;
        LogoutReason = reason;
        LogoutIpAddress = ip;
        LogoutUserAgent = userAgent;
        IsActiveSession = false;
    }
}
