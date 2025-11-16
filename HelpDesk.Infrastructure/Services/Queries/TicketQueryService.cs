using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Services.Queries
{
    public class TicketQueryService : ITicketQueryService
    {
        private readonly HelpDeskDbContext _context;

        public TicketQueryService(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TicketListItemDto>> GetTicketsAsync(
            TicketFilterRequest f,
            CancellationToken ct)
        {
            var page = Math.Max(1, f.Page);
            var pageSize = Math.Clamp(f.PageSize, 1, 500);

            var query = _context.Tickets.AsNoTracking();

            if (f.StatusId.HasValue)
                query = query.Where(t => t.StatusId == f.StatusId);

            if (f.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == f.CategoryId);

            if (f.TypeId.HasValue)
                query = query.Where(t => t.TypeId == f.TypeId);

            if (f.PriorityId.HasValue)
                query = query.Where(t => t.PriorityId == f.PriorityId);

            if (f.AssignedToEmployeeId.HasValue)
                query = query.Where(t => t.AssignedToEmployeeId == f.AssignedToEmployeeId);

            if (f.CreatedFrom.HasValue)
            {
                var from = f.CreatedFrom.Value.Date;
                query = query.Where(t => t.TicketCreatedAt >= from);
            }

            if (f.CreatedTo.HasValue)
            {
                var to = f.CreatedTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.TicketCreatedAt <= to);
            }

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(t => t.TicketCreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketListItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    StatusId = t.StatusId,
                    PriorityId = t.PriorityId,
                    CategoryId = t.CategoryId,
                    TypeId = t.TypeId,
                    CreatedById = t.CreatedById,
                    AssignedToEmployeeId = t.AssignedToEmployeeId,
                    TicketCreatedAt = t.TicketCreatedAt
                })
                .ToListAsync(ct);

            return new PagedResult<TicketListItemDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
