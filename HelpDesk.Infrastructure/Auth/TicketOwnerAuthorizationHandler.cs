using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.HelpDesk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace HelpDesk.Infrastructure.Auth
{
    /// <summary>
    /// Handler que verifica si un usuario puede acceder a un ticket específico.
    /// Usa permisos dinámicos (ViewAllTickets) o propiedad del recurso.
    /// </summary>
    public class TicketOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Ticket>
    {
        private readonly IRolePermissionService _permissions;

        public TicketOwnerAuthorizationHandler(IRolePermissionService permissions)
        {
            _permissions = permissions;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Ticket resource)
        {
            if (context.User == null || resource == null)
                return;

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return;

            // 1. Verificar si tiene permiso global para ver todos los tickets
            var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (await _permissions.HasPermissionAsync(roles, "ViewAllTickets"))
            {
                context.Succeed(requirement);
                return;
            }

            // 2. Si no tiene permiso global, verificar si es el dueño
            if (resource.CreatedById == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }

    /// <summary>
    /// Handler para TicketDto (usado en lecturas)
    /// </summary>
    public class TicketDtoAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Application.DTOs.TicketDto>
    {
        private readonly IRolePermissionService _permissions;

        public TicketDtoAuthorizationHandler(IRolePermissionService permissions)
        {
            _permissions = permissions;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Application.DTOs.TicketDto resource)
        {
            if (context.User == null || resource == null)
                return;

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return;

            // 1. Verificar si tiene permiso global
            var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (await _permissions.HasPermissionAsync(roles, "ViewAllTickets"))
            {
                context.Succeed(requirement);
                return;
            }

            // 2. Verificar propiedad
            if (resource.CreatedById == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }

    /// <summary>
    /// Operaciones que se pueden realizar sobre tickets
    /// </summary>
    public static class TicketOperations
    {
        public static OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
        public static OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
    }
}
