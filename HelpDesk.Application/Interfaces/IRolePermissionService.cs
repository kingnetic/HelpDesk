using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces
{
    public interface IRolePermissionService
    {
        Task<bool> HasPermissionAsync(IEnumerable<string> roleNames, string permission, CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> GetPermissionsForRoleAsync(string roleName, CancellationToken ct = default);
        Task AddPermissionToRoleAsync(string roleName, string permission, CancellationToken ct = default);
        Task RemovePermissionFromRoleAsync(string roleName, string permission, CancellationToken ct = default);
    }
}
