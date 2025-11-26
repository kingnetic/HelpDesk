using HelpDesk.Application.DTOs;

namespace HelpDesk.Application.Interfaces
{
    public interface ITicketQueryService
    {
        Task<PagedResult<TicketListItemDto>> GetTicketsAsync(TicketFilterRequest filter, CancellationToken ct);
        Task<TicketDto?> GetTicketByIdAsync(int id, CancellationToken ct);
    }
}
