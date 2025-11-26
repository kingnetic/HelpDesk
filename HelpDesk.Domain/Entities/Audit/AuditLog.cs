using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities.Audit
{
    /// <summary>
    /// Registro unificado de auditoría para todo el sistema.
    /// Combina auditoría de seguridad HTTP y operaciones de negocio.
    /// </summary>
    public class AuditLog : BaseEntity
    {
        // Información del usuario
        public string? UserId { get; private set; }

        // Tipo de auditoría: "HTTP" o "Business"
        public string AuditType { get; private set; } = string.Empty;

        // Para auditoría HTTP
        public string? HttpMethod { get; private set; }
        public string? HttpPath { get; private set; }
        public int? HttpStatusCode { get; private set; }
        public long? DurationMs { get; private set; }

        // Para auditoría de negocio
        public int? TicketId { get; private set; }
        public string? Action { get; private set; }
        public string? Detail { get; private set; }

        // Información común
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public string? ErrorMessage { get; private set; }

        private AuditLog() { }

        // Constructor para auditoría HTTP
        public static AuditLog CreateHttpLog(
            string? userId,
            string method,
            string path,
            int statusCode,
            string? ipAddress,
            string? userAgent,
            long durationMs,
            string? errorMessage = null)
        {
            return new AuditLog
            {
                UserId = userId,
                AuditType = "HTTP",
                HttpMethod = method,
                HttpPath = path,
                HttpStatusCode = statusCode,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DurationMs = durationMs,
                ErrorMessage = errorMessage
            };
        }

        // Constructor para auditoría de negocio
        public static AuditLog CreateBusinessLog(
            int ticketId,
            int userId,
            string action,
            string? detail,
            string? ipAddress,
            string? userAgent)
        {
            return new AuditLog
            {
                UserId = userId.ToString(),
                AuditType = "Business",
                TicketId = ticketId,
                Action = action,
                Detail = detail,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
        }
    }
}
