namespace HelpDesk.Application.DTOs
{
    public class TicketListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int CategoryId { get; set; }
        public int? TypeId { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToEmployeeId { get; set; }
        public DateTime TicketCreatedAt { get; set; }
    }
}
