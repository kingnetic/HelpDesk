using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using HelpDesk.Infrastructure.Persistence;
using HelpDesk.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly HelpDeskDbContext _context;

        public AuditService(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int ticketId, int userId, string action, string? detail, string? ip, string? userAgent)
        {
            var log = new TicketAuditLog(ticketId, userId, action, detail, ip, userAgent);

            _context.Set<TicketAuditLog>().Add(log);
            await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<AuditLogDto>> GetLogsAsync(AuditLogFilterRequest f)
        {
            var query = _context.Set<TicketAuditLog>().AsNoTracking().AsQueryable();

            if (f.TicketId.HasValue) query = query.Where(x => x.TicketId == f.TicketId);
            if (f.UserId.HasValue) query = query.Where(x => x.UserId == f.UserId);
            if (!string.IsNullOrEmpty(f.Action)) query = query.Where(x => x.Action == f.Action);
            if (f.From.HasValue) query = query.Where(x => x.CreatedAt >= f.From.Value);
            if (f.To.HasValue) query = query.Where(x => x.CreatedAt <= f.To.Value);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((f.Page - 1) * f.PageSize)
                .Take(f.PageSize)
                .Select(x => new AuditLogDto(
                    x.Id,
                    x.TicketId,
                    x.UserId,
                    x.Action,
                    x.Detail,
                    x.IpAddress,
                    x.UserAgent,
                    x.CreatedAt))
                .ToListAsync();

            return new PagedResult<AuditLogDto>
            {
                Items = items,
                TotalCount = total,
                Page = f.Page,
                PageSize = f.PageSize
            };
        }
    }
}
