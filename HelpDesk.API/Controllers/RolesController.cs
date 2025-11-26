using HelpDesk.Application.Interfaces;
using HelpDesk.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly RoleManager<Role> _roleManager;

        public RolesController(IRolePermissionService rolePermissionService, RoleManager<Role> roleManager)
        {
            _rolePermissionService = rolePermissionService;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Obtiene todos los roles disponibles en el sistema.
        /// Requiere permiso 'ManageRoles'.
        /// </summary>
        /// <returns>Lista de roles.</returns>
        [Authorize(Policy = "ManageRoles")]
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        /// <summary>
        /// Obtiene los permisos asignados a un rol específico.
        /// Requiere permiso 'ManageRoles'.
        /// </summary>
        /// <param name="roleName">Nombre del rol.</param>
        /// <returns>Lista de permisos (strings).</returns>
        [Authorize(Policy = "ManageRoles")]
        [HttpGet("{roleName}/permissions")]
        public async Task<IActionResult> GetPermissions(string roleName)
        {
            var permissions = await _rolePermissionService.GetPermissionsForRoleAsync(roleName);
            return Ok(permissions);
        }

        /// <summary>
        /// Asigna un nuevo permiso a un rol.
        /// Requiere permiso 'ManageRoles'.
        /// </summary>
        /// <param name="roleName">Nombre del rol.</param>
        /// <param name="request">Nombre del permiso a añadir.</param>
        [Authorize(Policy = "ManageRoles")]
        [HttpPost("{roleName}/permissions")]
        public async Task<IActionResult> AddPermission(string roleName, [FromBody] AddPermissionRequest request)
        {
            await _rolePermissionService.AddPermissionToRoleAsync(roleName, request.Permission);
            return Ok();
        }

        /// <summary>
        /// Elimina un permiso de un rol.
        /// Requiere permiso 'ManageRoles'.
        /// </summary>
        /// <param name="roleName">Nombre del rol.</param>
        /// <param name="permission">Nombre del permiso a eliminar.</param>
        [Authorize(Policy = "ManageRoles")]
        [HttpDelete("{roleName}/permissions/{permission}")]
        public async Task<IActionResult> RemovePermission(string roleName, string permission)
        {
            await _rolePermissionService.RemovePermissionFromRoleAsync(roleName, permission);
            return NoContent();
        }

        // --- Role Management ---

        /// <summary>
        /// Crea un nuevo rol en el sistema.
        /// Requiere permiso 'ManageRoles'.
        /// </summary>
        /// <param name="request">Nombre del nuevo rol.</param>
        [Authorize(Policy = "ManageRoles")]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (await _roleManager.RoleExistsAsync(request.RoleName))
                return BadRequest("Role already exists");

            var result = await _roleManager.CreateAsync(new Role { Name = request.RoleName });
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        public record AddPermissionRequest(string Permission);
        public record CreateRoleRequest(string RoleName);
    }
}
