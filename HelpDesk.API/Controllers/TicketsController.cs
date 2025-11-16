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
        public TicketsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Obtiene tickets filtrados y paginados.
        /// Query parameters: statusId, categoryId, typeId, priorityId, assignedToEmployeeId, createdFrom, createdTo, page, pageSize
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] TicketFilterRequest filter)
        {
            var result = await _mediator.Send(new GetTicketsQuery { Filter = filter });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [Authorize]
        [Authorize]
        [HttpPost("{ticketId:int}/assign/{employeeId:int}")]
        public async Task<IActionResult> AssignTicket(int ticketId, int employeeId)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var agent = Request.Headers["User-Agent"].ToString();

            var cmd = new AssignTicketCommand(ticketId, employeeId, ip, agent);

            var result = await _mediator.Send(cmd);
            return Ok(result);
        }


        [Authorize]
        [HttpPost("{id}/reply")]
        public async Task<IActionResult> Reply(int id, ReplyTicketRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new ReplyTicketCommand(id, userId, request.Comment);

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        public record ReplyTicketRequest(string Comment);


        [Authorize]
        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(int id, [FromBody] CloseTicketCommand cmd)
        {
            if (id != cmd.TicketId) return BadRequest();
            await _mediator.Send(cmd);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Query handler not implemented in this minimal scaffold; return 501 for now
            return StatusCode(501);
        }
    }
}
