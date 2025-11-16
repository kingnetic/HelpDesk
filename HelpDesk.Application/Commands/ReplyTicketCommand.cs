using MediatR;
using HelpDesk.Application.DTOs.Tickets;

namespace HelpDesk.Application.Commands
{
    public record ReplyTicketCommand(int TicketId, int UserId, string Comment) : IRequest<ReplyTicketResultDto>;
}
