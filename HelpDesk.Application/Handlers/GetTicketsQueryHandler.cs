using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Queries;
using MediatR;

namespace HelpDesk.Application.Handlers
{
    public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, PagedResult<TicketListItemDto>>
    {
        private readonly ITicketQueryService _queries;

        public GetTicketsQueryHandler(ITicketQueryService queries)
        {
            _queries = queries;
        }

        public async Task<PagedResult<TicketListItemDto>> Handle(GetTicketsQuery request, CancellationToken ct)
        {
            return await _queries.GetTicketsAsync(request.Filter, ct);
        }
    }
}
