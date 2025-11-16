using HelpDesk.Application.DTOs;
using MediatR;

namespace HelpDesk.Application.Queries
{
    public class GetTicketsQuery : IRequest<PagedResult<TicketListItemDto>>
    {
        public TicketFilterRequest Filter { get; init; } = new();
    }
}
