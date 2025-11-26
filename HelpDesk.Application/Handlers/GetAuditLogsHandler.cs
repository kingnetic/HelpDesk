using HelpDesk.Application.DTOs;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Handlers
{
    public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
    {
        private readonly IAuditService _auditService;

        public GetAuditLogsHandler(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            return await _auditService.GetLogsAsync(request.Filter);
        }
    }
}
