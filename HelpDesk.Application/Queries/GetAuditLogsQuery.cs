using HelpDesk.Application.DTOs;
using MediatR;

namespace HelpDesk.Application.Queries
{
    public class GetAuditLogsQuery : IRequest<PagedResult<AuditLogDto>>
    {
        public AuditLogFilterRequest Filter { get; set; } = new();
    }
}
