using MediatR;
using HelpDesk.Application.Commands;
using HelpDesk.Application.DTOs.Tickets;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;

namespace HelpDesk.Application.Handlers
{
    public class ReplyTicketHandler : IRequestHandler<ReplyTicketCommand, ReplyTicketResultDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;

        public ReplyTicketHandler(IUnitOfWork uow, IEmailSender emailSender, IUserService userService)
        {
            _uow = uow;
            _emailSender = emailSender;
            _userService = userService;
        }

        public async Task<ReplyTicketResultDto> Handle(ReplyTicketCommand request, CancellationToken cancellationToken)
        {
            var repo = _uow.Repository<Ticket>();
            var ticket = await repo.GetByIdAsync(request.TicketId)
                ?? throw new Exception("Ticket not found");

            // --- Dominio: Agregar comentario ---
            ticket.Reply(request.UserId, request.Comment);

            repo.Update(ticket);
            await _uow.SaveChangesAsync(cancellationToken);

            // Email del creador
            var creatorEmail = await _userService.GetEmailByUserIdAsync(ticket.CreatedById);

            if (!string.IsNullOrWhiteSpace(creatorEmail))
            {
                await _emailSender.SendTicketReplyNotificationAsync(
                    creatorEmail,
                    ticket.Id,
                    request.Comment
                );
            }

            return new ReplyTicketResultDto
            {
                TicketId = ticket.Id,
                UserId = request.UserId,
                Comment = request.Comment,
                RepliedAt = DateTime.UtcNow
            };
        }
    }
}
