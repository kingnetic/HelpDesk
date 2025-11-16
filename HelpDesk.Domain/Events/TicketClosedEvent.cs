using System;
using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Events
{
    public class TicketClosedEvent : IDomainEvent
    {
        public int TicketId { get; }
        public int ClosedByUserId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public TicketClosedEvent(int ticketId, int closedByUserId) { TicketId = ticketId; ClosedByUserId = closedByUserId; }
    }
}
