using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Commands;
using HelpDesk.Domain.Entities.HelpDesk;

namespace HelpDesk.Application.Handlers
{
    public class CreateTicketHandler : IRequestHandler<CreateTicketCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public CreateTicketHandler(IUnitOfWork uow) { _uow = uow; }

        public async Task<int> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new Ticket(request.Title, request.Description, request.CreatedById, request.CategoryId, request.PriorityId, request.InitialStatusId, request.TypeId);
            await _uow.Repository<Ticket>().AddAsync(ticket);
            await _uow.SaveChangesAsync(cancellationToken);

            // Domain events could be dispatched here via MediatR (not implemented in this minimal handler)
            return ticket.Id;
        }
    }
}
