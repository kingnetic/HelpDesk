using System;
using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Events
{
    public class TicketAssignedEvent : IDomainEvent
    {
        public int TicketId { get; }
        public int EmployeeId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public TicketAssignedEvent(int ticketId, int employeeId) { TicketId = ticketId; EmployeeId = employeeId; }
    }
}
