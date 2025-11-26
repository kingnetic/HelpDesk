using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto?>
    {
        private readonly ITicketQueryService _queryService;

        public GetTicketByIdQueryHandler(ITicketQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<TicketDto?> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            return await _queryService.GetTicketByIdAsync(request.Id, cancellationToken);
        }
    }
}
