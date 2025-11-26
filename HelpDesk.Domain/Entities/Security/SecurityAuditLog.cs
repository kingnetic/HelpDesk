using HelpDesk.Domain.Entities.Common;

namespace HelpDesk.Domain.Entities.Security
{
    /// <summary>
    /// Registro de auditoría de seguridad para todos los requests HTTP.
    /// Complementa a TicketAuditLog que solo audita operaciones de negocio.
    /// </summary>
    public class SecurityAuditLog : BaseEntity
    {
        public string? UserId { get; private set; }
        public string Method { get; private set; } = string.Empty;
        public string Path { get; private set; } = string.Empty;
        public int StatusCode { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public long DurationMs { get; private set; }
        public string? ErrorMessage { get; private set; }

        private SecurityAuditLog() { }

        public SecurityAuditLog(
            string? userId,
            string method,
            string path,
            int statusCode,
            string? ipAddress,
            string? userAgent,
            long durationMs,
            string? errorMessage = null)
        {
            UserId = userId;
            Method = method;
            Path = path;
            StatusCode = statusCode;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            DurationMs = durationMs;
            ErrorMessage = errorMessage;
        }
    }
}
