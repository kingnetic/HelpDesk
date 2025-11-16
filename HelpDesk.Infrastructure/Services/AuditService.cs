using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using HelpDesk.Infrastructure.Persistence;

namespace HelpDesk.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly HelpDeskDbContext _context;

        public AuditService(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            int ticketId,
            int userId,
            string action,
            string? detail,
            string? ip,
            string? userAgent)
        {
            var log = new TicketAuditLog(
                ticketId,
                userId,
                action,
                detail,
                ip,
                userAgent
            );

            _context.Set<TicketAuditLog>().Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
