using System;
using System.Collections.Generic;

namespace HelpDesk.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int CategoryId { get; set; }
        public int? TypeId { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToEmployeeId { get; set; }
        public DateTime TicketCreatedAt { get; set; }
        public List<TicketCommentDto> Comments { get; set; } = new();
        public List<TicketStatusHistoryDto> History { get; set; } = new();
    }

    public class TicketCommentDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAtLocal { get; set; }
    }
}
