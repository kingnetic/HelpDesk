using HelpDesk.Application.Commands;
using HelpDesk.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HelpDesk.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios registrados.
        /// Requiere permiso 'ManageUsers'.
        /// </summary>
        /// <returns>Lista de usuarios con sus roles.</returns>
        [Authorize(Policy = "ManageUsers")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// Requiere permiso 'ManageUsers'.
        /// </summary>
        [Authorize(Policy = "ManageUsers")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// Requiere permiso 'ManageUsers'.
        /// </summary>
        [Authorize(Policy = "ManageUsers")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Elimina un usuario.
        /// Requiere permiso 'ManageUsers'.
        /// </summary>
        [Authorize(Policy = "ManageUsers")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Asigna roles a un usuario específico.
        /// Requiere permiso 'ManageUsers'.
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="request">Lista de roles a asignar</param>
        [Authorize(Policy = "ManageUsers")]
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] AssignRolesRequest request)
        {
            var command = new UpdateUserCommand(id, request.Email, request.FullName, request.Roles);
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(new { message = "Roles asignados correctamente" });
        }

        public record AssignRolesRequest(string Email, string FullName, List<string> Roles);
    }
}
