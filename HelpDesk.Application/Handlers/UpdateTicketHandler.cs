using HelpDesk.Application.Commands;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class UpdateTicketHandler : IRequestHandler<UpdateTicketCommand, bool>
    {
        private readonly IGenericRepository<Ticket> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTicketHandler(IGenericRepository<Ticket> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdAsync(request.TicketId);
            if (ticket == null) return false;

            ticket.UpdateDetails(request.Title, request.Description, request.PriorityId, request.CategoryId, request.TypeId);

            _repository.Update(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
