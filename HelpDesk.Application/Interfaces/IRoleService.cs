using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.Application.Interfaces
{
    public interface IRoleService
    {
        Task<List<string>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> UpdateRoleAsync(string roleName, string newRoleName);
        Task<bool> DeleteRoleAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
