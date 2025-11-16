using MediatR;

namespace HelpDesk.Application.Commands
{
    public record CloseTicketCommand(int TicketId, int ClosedByUserId, int ClosedStatusId) : IRequest;
}
