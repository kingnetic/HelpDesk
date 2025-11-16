namespace HelpDesk.Application.DTOs.Tickets
{
    public class ReplyTicketResultDto
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime RepliedAt { get; set; }
    }
}
