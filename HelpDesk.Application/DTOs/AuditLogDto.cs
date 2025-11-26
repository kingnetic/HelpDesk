namespace HelpDesk.Application.DTOs
{
    public record AuditLogDto(
        int Id,
        int TicketId,
        int UserId,
        string Action,
        string? Detail,
        string? IpAddress,
        string? UserAgent,
        System.DateTime CreatedAt
    );

    public class AuditLogFilterRequest
    {
        public int? TicketId { get; set; }
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public System.DateTime? From { get; set; }
        public System.DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
