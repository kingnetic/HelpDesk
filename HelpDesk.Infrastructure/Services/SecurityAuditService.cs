using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Audit;
using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.Security;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de auditoría de seguridad.
    /// Registra todos los requests HTTP para trazabilidad y seguridad.
    /// </summary>
    public class SecurityAuditService : ISecurityAuditService
    {
        private readonly HelpDeskDbContext _context;

        public SecurityAuditService(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task LogRequestAsync(
            string? userId,
            string method,
            string path,
            int statusCode,
            string? ipAddress,
            string? userAgent,
            long durationMs,
            string? errorMessage = null)
        {
            var log = new SecurityAuditLog(
                userId,
                method,
                path,
                statusCode,
                ipAddress,
                userAgent,
                durationMs,
                errorMessage);

            _context.Set<SecurityAuditLog>().Add(log);

            // Guardar de forma asíncrona sin bloquear el request
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<SecurityAuditLogDto>> GetLogsAsync(SecurityAuditFilterRequest filter)
        {
            var query = _context.Set<SecurityAuditLog>().AsNoTracking().AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(x => x.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.Method))
                query = query.Where(x => x.Method == filter.Method);

            if (!string.IsNullOrEmpty(filter.Path))
                query = query.Where(x => x.Path.Contains(filter.Path));

            if (filter.StatusCode.HasValue)
                query = query.Where(x => x.StatusCode == filter.StatusCode);

            if (filter.From.HasValue)
                query = query.Where(x => x.CreatedAt >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(x => x.CreatedAt <= filter.To.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new SecurityAuditLogDto(
                    x.Id,
                    x.UserId,
                    x.Method,
                    x.Path,
                    x.StatusCode,
                    x.IpAddress,
                    x.UserAgent,
                    x.DurationMs,
                    x.ErrorMessage,
                    x.CreatedAt))
                .ToListAsync();

            return new PagedResult<SecurityAuditLogDto>
            {
                Items = items,
                TotalCount = total,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }
    }
}
