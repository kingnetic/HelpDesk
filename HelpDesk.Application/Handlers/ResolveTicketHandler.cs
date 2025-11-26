using HelpDesk.Application.Commands;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class ResolveTicketHandler : IRequestHandler<ResolveTicketCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;
        private readonly IEmailSender _email;

        public ResolveTicketHandler(IUnitOfWork uow, IUserService userService, IEmailSender email)
        {
            _uow = uow;
            _userService = userService;
            _email = email;
        }

        public async Task Handle(ResolveTicketCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.Repository<Ticket>();
            var ticket = await repo.GetByIdAsync(request.TicketId)
                ?? throw new Exception("Ticket not found.");

            ticket.Resolve(request.ResolvedByUserId, request.ResolvedStatusId);

            repo.Update(ticket);
            await _uow.SaveChangesAsync(cancellationToken);

            // Notify creator
            var creatorEmail = await _userService.GetEmailByUserIdAsync(ticket.CreatedById);
            if (!string.IsNullOrWhiteSpace(creatorEmail))
            {
                await _email.SendTicketResolvedNotificationAsync(creatorEmail, ticket.Id);
            }
        }
    }
}
