using HelpDesk.Application.Commands;
using MediatR;

namespace HelpDesk.Application.Commands
{
    public record ResolveTicketCommand(int TicketId, int ResolvedByUserId, int ResolvedStatusId) : IRequest;
}
