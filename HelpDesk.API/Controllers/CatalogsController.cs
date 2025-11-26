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
    public class CatalogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene todos los ítems de catálogo (Prioridades, Categorías, Estados, Tipos).
        /// </summary>
        /// <returns>Un objeto con listas de cada tipo de catálogo.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCatalogs()
        {
            var result = await _mediator.Send(new GetCatalogsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo catálogo o valor.
        /// Requiere permiso 'ManageCatalogs'.
        /// </summary>
        [Authorize(Policy = "ManageCatalogs")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCatalogCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { Id = id });
        }

        /// <summary>
        /// Actualiza un catálogo existente.
        /// Requiere permiso 'ManageCatalogs'.
        /// </summary>
        [Authorize(Policy = "ManageCatalogs")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCatalogCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Elimina un catálogo.
        /// Requiere permiso 'ManageCatalogs'.
        /// </summary>
        [Authorize(Policy = "ManageCatalogs")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCatalogCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
