using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.Auth;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly HelpDeskDbContext _context;

        public UserSessionService(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateSessionAsync(int userId, Guid jwtId, DateTime expiresAt, string? ip, string? userAgent)
        {
            var session = new UserSession(userId, jwtId, expiresAt, ip, userAgent);
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();
            return session.Id;
        }

        public async Task CloseSessionAsync(int sessionId, string? reason, string? ip, string? userAgent)
        {
            var session = await _context.UserSessions.FindAsync(sessionId);
            if (session == null) return;
            session.Close(reason, ip, userAgent);
            await _context.SaveChangesAsync();
        }

        public async Task CloseSessionByJwtIdAsync(Guid jwtId, string? reason, string? ip, string? userAgent)
        {
            var session = await _context.UserSessions
                .Where(s => s.JwtId == jwtId && s.IsActiveSession)
                .OrderByDescending(s => s.LoginAt)
                .FirstOrDefaultAsync();

            if (session == null) return;
            session.Close(reason, ip, userAgent);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetActiveSessionIdForUserAsync(int userId)
        {
            var s = await _context.UserSessions
                .Where(x => x.UserId == userId && x.IsActiveSession)
                .OrderByDescending(x => x.LoginAt)
                .FirstOrDefaultAsync();

            return s?.Id;
        }
    }
}
