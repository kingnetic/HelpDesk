using System;
using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Events
{
    public class TicketRepliedEvent : IDomainEvent
    {
        public int TicketId { get; }
        public int UserId { get; }
        public string Comment { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public TicketRepliedEvent(int ticketId, int userId, string comment) { TicketId = ticketId; UserId = userId; Comment = comment; }
    }
}
