using System;

namespace HelpDesk.Application.DTOs
{
    public class TicketStatusHistoryDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int StatusId { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
