using MediatR;

namespace HelpDesk.Application.Commands
{
    public class DeleteTicketCommand : IRequest<bool>
    {
        public int TicketId { get; set; }

        public DeleteTicketCommand(int ticketId)
        {
            TicketId = ticketId;
        }
    }
}
