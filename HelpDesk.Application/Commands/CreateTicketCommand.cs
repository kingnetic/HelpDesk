using MediatR;

namespace HelpDesk.Application.Commands
{
    public record CreateTicketCommand(string Title, string Description, int CreatedById, int CategoryId, int PriorityId, int InitialStatusId, int? TypeId) : IRequest<int>;
}
