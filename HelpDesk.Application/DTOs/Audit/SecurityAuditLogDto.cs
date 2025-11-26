namespace HelpDesk.Application.DTOs.Audit
{
    public record SecurityAuditLogDto(
        int Id,
        string? UserId,
        string Method,
        string Path,
        int StatusCode,
        string? IpAddress,
        string? UserAgent,
        long DurationMs,
        string? ErrorMessage,
        DateTime CreatedAt);

    public record SecurityAuditFilterRequest
    {
        public string? UserId { get; set; }
        public string? Method { get; set; }
        public string? Path { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
