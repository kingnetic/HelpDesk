using HelpDesk.Application.Commands;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class RestoreTicketHandler : IRequestHandler<RestoreTicketCommand, bool>
    {
        private readonly IGenericRepository<Ticket> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RestoreTicketHandler(IGenericRepository<Ticket> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RestoreTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _repository.GetByIdAsync(request.TicketId);
            if (ticket == null) return false;

            // Restaurar ticket - quitar marca de eliminado
            ticket.Restore();
            _repository.Update(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
