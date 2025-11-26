using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Audit;
using HelpDesk.Domain.Entities.Security;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces
{
    /// <summary>
    /// Servicio para auditoría de seguridad HTTP.
    /// Complementa a IAuditService que solo audita operaciones de negocio.
    /// </summary>
    public interface ISecurityAuditService
    {
        /// <summary>
        /// Registra un request HTTP en el log de seguridad.
        /// </summary>
        Task LogRequestAsync(
            string? userId,
            string method,
            string path,
            int statusCode,
            string? ipAddress,
            string? userAgent,
            long durationMs,
            string? errorMessage = null);

        /// <summary>
        /// Obtiene logs de seguridad con filtros y paginación.
        /// </summary>
        Task<PagedResult<SecurityAuditLogDto>> GetLogsAsync(SecurityAuditFilterRequest filter);
    }
}
