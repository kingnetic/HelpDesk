using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Auth
{
    public class DynamicPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DynamicPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Primero, verificar si la política ya está definida
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
            {
                return policy;
            }

            // Si no, crear dinámicamente una política que requiere un permiso con el mismo nombre
            // Esto asume que para cada política "X", existe un permiso "X"
            return new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }
    }
}
