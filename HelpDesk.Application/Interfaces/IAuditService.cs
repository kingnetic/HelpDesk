namespace HelpDesk.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int ticketId, int userId, string action, string? detail, string? ip, string? userAgent);
    }
}
