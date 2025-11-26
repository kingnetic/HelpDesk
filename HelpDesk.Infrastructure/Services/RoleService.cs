using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Application.Interfaces;
using HelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles
                .Where(r => r.Name != null)
                .Select(r => r.Name!)
                .ToListAsync();
        }

        public async Task<bool> CreateRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName)) return false;
            var result = await _roleManager.CreateAsync(new Role { Name = roleName });
            return result.Succeeded;
        }

        public async Task<bool> UpdateRoleAsync(string roleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;

            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
