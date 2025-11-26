using MediatR;

namespace HelpDesk.Application.Commands
{
    public class UpdateTicketCommand : IRequest<bool>
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PriorityId { get; set; }
        public int CategoryId { get; set; }
        public int? TypeId { get; set; }

        public UpdateTicketCommand(int ticketId, int userId, string title, string description, int priorityId, int categoryId, int? typeId)
        {
            TicketId = ticketId;
            UserId = userId;
            Title = title;
            Description = description;
            PriorityId = priorityId;
            CategoryId = categoryId;
            TypeId = typeId;
        }
    }
}
