using HelpDesk.Application.Commands;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class DeleteTicketHandler : IRequestHandler<DeleteTicketCommand, bool>
    {
        private readonly IGenericRepository<Ticket> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTicketHandler(IGenericRepository<Ticket> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdAsync(request.TicketId);
            if (ticket == null) return false;

            // Soft delete - marcar como eliminado sin borrar físicamente
            ticket.Delete();
            _repository.Update(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
