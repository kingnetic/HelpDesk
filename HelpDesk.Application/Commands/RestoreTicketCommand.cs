using MediatR;

namespace HelpDesk.Application.Commands
{
    public record RestoreTicketCommand(int TicketId) : IRequest<bool>;
}
