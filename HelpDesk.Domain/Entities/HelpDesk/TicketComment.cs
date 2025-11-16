using System;
using HelpDesk.Domain.Entities.Common;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Domain.Entities.HelpDesk
{
    public class TicketComment : BaseEntity
    {
        public int TicketId { get; private set; }
        public int UserId { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAtLocal { get; private set; } = DateTime.UtcNow;

        public TicketComment() { }

        public TicketComment(int ticketId, int userId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) throw new DomainException("Comment cannot be empty.");
            TicketId = ticketId;
            UserId = userId;
            Comment = comment;
        }
    }
}
