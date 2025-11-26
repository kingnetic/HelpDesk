using HelpDesk.Application.Commands;
using HelpDesk.Application.DTOs;
using HelpDesk.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpDesk.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;

        public TicketsController(IMediator mediator, IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Obtiene tickets filtrados y paginados.
        /// Query parameters: statusId, categoryId, typeId, priorityId, assignedToEmployeeId, createdFrom, createdTo, page, pageSize
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] TicketFilterRequest filter)
        {
            // Verificar si tiene permiso para ver todos los tickets
            var authResult = await _authorizationService.AuthorizeAsync(User, "ViewAllTickets");
            
            // Si NO tiene permiso de ver todos, filtrar por su ID (comportamiento para Customer)
            if (!authResult.Succeeded)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                filter.CreatedById = userId;
            }

            var result = await _mediator.Send(new GetTicketsQuery { Filter = filter });
            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo ticket.
        /// </summary>
        /// <param name="command">Datos del ticket a crear.</param>
        /// <returns>El ID del ticket creado.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketCommand command)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var secureCommand = command with { CreatedById = userId };
            
            var id = await _mediator.Send(secureCommand);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        /// <summary>
        /// Asigna un ticket a un empleado de soporte.
        /// Requiere permiso 'AssignTicket'.
        /// </summary>
        /// <param name="ticketId">ID del ticket.</param>
        /// <param name="employeeId">ID del empleado.</param>
        [Authorize(Policy = "AssignTicket")]
        [HttpPost("{ticketId:int}/assign/{employeeId:int}")]
        public async Task<IActionResult> AssignTicket(int ticketId, int employeeId)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var agent = Request.Headers["User-Agent"].ToString();

            var cmd = new AssignTicketCommand(ticketId, employeeId, ip, agent);

            var result = await _mediator.Send(cmd);
            return Ok(result);
        }


        /// <summary>
        /// Añade una respuesta/comentario a un ticket.
        /// </summary>
        /// <param name="id">ID del ticket.</param>
        /// <param name="request">Contenido del comentario.</param>
        [Authorize]
        [HttpPost("{id}/reply")]
        public async Task<IActionResult> Reply(int id, ReplyTicketRequest request)
        {
            // Verificar acceso
            var ticket = await _mediator.Send(new GetTicketByIdQuery(id));
            if (ticket == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, ticket, HelpDesk.Infrastructure.Auth.TicketOperations.Read);
            if (!authResult.Succeeded) return Forbid();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new ReplyTicketCommand(id, userId, request.Comment);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        public record ReplyTicketRequest(string Comment);


        /// <summary>
        /// Cierra un ticket definitivamente.
        /// Requiere permiso 'CloseTicket'.
        /// </summary>
        /// <param name="id">ID del ticket.</param>
        /// <param name="cmd">Comando de cierre.</param>
        [Authorize(Policy = "CloseTicket")]
        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(int id, [FromBody] CloseTicketCommand cmd)
        {
            if (id != cmd.TicketId) return BadRequest();
            await _mediator.Send(cmd);
            return NoContent();
        }

        /// <summary>
        /// Marca un ticket como resuelto.
        /// Requiere permiso 'ResolveTicket'.
        /// </summary>
        /// <param name="id">ID del ticket.</param>
        /// <param name="request">Estado de resolución.</param>
        [Authorize(Policy = "ResolveTicket")]
        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveTicketRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cmd = new ResolveTicketCommand(id, userId, request.StatusId);
            await _mediator.Send(cmd);
            return NoContent();
        }

        public record ResolveTicketRequest(int StatusId);

        /// <summary>
        /// Obtiene los detalles de un ticket por su ID.
        /// </summary>
        /// <param name="id">ID del ticket.</param>
        /// <returns>Detalles del ticket incluyendo comentarios e historial.</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetTicketByIdQuery(id));
            if (result == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, result, HelpDesk.Infrastructure.Auth.TicketOperations.Read);
            if (!authResult.Succeeded) return Forbid();

            return Ok(result);
        }

        /// <summary>
        /// Actualiza la información básica de un ticket.
        /// </summary>
        /// <param name="id">ID del ticket.</param>
        /// <param name="request">Datos a actualizar.</param>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketRequest request)
        {
            // Verificar acceso
            var ticket = await _mediator.Send(new GetTicketByIdQuery(id));
            if (ticket == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, ticket, HelpDesk.Infrastructure.Auth.TicketOperations.Update);
            if (!authResult.Succeeded) return Forbid();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new UpdateTicketCommand(
                id, 
                userId, 
                request.Title, 
                request.Description, 
                request.PriorityId, 
                request.CategoryId, 
                request.TypeId);

            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }

        public record UpdateTicketRequest(string Title, string Description, int PriorityId, int CategoryId, int? TypeId);

        /// <summary>
        /// Elimina un ticket del sistema (soft delete).
        /// Requiere permiso 'DeleteTicket'.
        /// </summary>
        /// <param name="id">ID del ticket a eliminar.</param>
        [Authorize(Policy = "DeleteTicket")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteTicketCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Restaura un ticket eliminado (soft delete).
        /// Requiere permiso 'DeleteTicket'.
        /// </summary>
        /// <param name="id">ID del ticket a restaurar.</param>
        [Authorize(Policy = "DeleteTicket")]
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _mediator.Send(new RestoreTicketCommand(id));
            if (!result) return NotFound(new { message = "Ticket no encontrado" });
            return Ok(new { message = "Ticket restaurado correctamente" });
        }
    }
}
