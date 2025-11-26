using HelpDesk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IRolePermissionService _rpSvc;

        public PermissionHandler(IRolePermissionService rpSvc)
        {
            _rpSvc = rpSvc;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User == null) return;

            // Identity pone los roles en claims de tipo ClaimTypes.Role
            var roleClaims = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (!roleClaims.Any()) return;

            var allowed = await _rpSvc.HasPermissionAsync(roleClaims, requirement.Permission);
            if (allowed)
            {
                context.Succeed(requirement);
            }
        }
    }
}
