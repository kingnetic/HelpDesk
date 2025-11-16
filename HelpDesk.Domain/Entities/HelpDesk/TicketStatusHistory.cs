using System;
using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities.HelpDesk
{
    public class TicketStatusHistory : BaseEntity
    {
        public int TicketId { get; private set; }
        public Ticket Ticket { get; private set; }   // <-- Navegación necesaria

        public int StatusId { get; private set; }
        public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;

        public TicketStatusHistory() { }

        public TicketStatusHistory(int ticketId, int statusId)
        {
            TicketId = ticketId;
            StatusId = statusId;
        }
    }
}
