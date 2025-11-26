namespace HelpDesk.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int ticketId, int userId, string action, string? detail, string? ip, string? userAgent);
        Task<HelpDesk.Application.DTOs.PagedResult<HelpDesk.Application.DTOs.AuditLogDto>> GetLogsAsync(HelpDesk.Application.DTOs.AuditLogFilterRequest filter);
    }
}
