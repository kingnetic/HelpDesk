using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities.HelpDesk
{
    public class TicketAuditLog : BaseEntity
    {
        public int TicketId { get; private set; }
        public int UserId { get; private set; }
        public string Action { get; private set; } = string.Empty;
        public string? Detail { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }

        private TicketAuditLog() { }

        public TicketAuditLog(
            int ticketId,
            int userId,
            string action,
            string? detail,
            string? ip,
            string? userAgent)
        {
            TicketId = ticketId;
            UserId = userId;
            Action = action;
            Detail = detail;
            IpAddress = ip;
            UserAgent = userAgent;
        }
    }
}
