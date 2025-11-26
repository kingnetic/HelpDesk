using HelpDesk.Application.DTOs;
using HelpDesk.Application.DTOs.Audit;
using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HelpDesk.API.DTOs;

namespace HelpDesk.Api.Controllers
{
    /// <summary>
    /// Controlador para consultar logs de auditoría.
    /// Incluye auditoría de seguridad (HTTP) y de negocio (Tickets).
    /// Requiere permiso 'ViewAuditLogs' (solo Admin).
    /// </summary>
    [Authorize(Policy = "ViewAuditLogs")]
    [ApiController]
    [Route("api/audit")]
    public class AuditController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISecurityAuditService _securityAudit;

        public AuditController(IMediator mediator, ISecurityAuditService securityAudit)
        {
            _mediator = mediator;
            _securityAudit = securityAudit;
        }

        /// <summary>
        /// Obtiene logs de auditoría de seguridad (HTTP requests).
        /// Requiere permiso 'ViewAuditLogs'.
        /// </summary>
        /// <param name="filter">Filtros opcionales: userId, method, path, statusCode, from, to, page, pageSize</param>
        [HttpGet("security")]
        public async Task<IActionResult> GetSecurityLogs([FromQuery] SecurityAuditFilterRequest filter)
        {
            var result = await _securityAudit.GetLogsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene logs de auditoría de negocio (operaciones de tickets).
        /// Requiere permiso 'ViewAuditLogs'.
        /// </summary>
        /// <param name="filter">Filtros de búsqueda.</param>
        /// <returns>Lista paginada de logs.</returns>
        [HttpGet("business")]
        public async Task<IActionResult> GetBusinessLogs([FromQuery] AuditLogFilterRequest filter)
        {
            var result = await _mediator.Send(new GetAuditLogsQuery { Filter = filter });
            return Ok(result);
        }
    }
}
