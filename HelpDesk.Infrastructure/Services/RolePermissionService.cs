using HelpDesk.Application.Interfaces;
using HelpDesk.Domain.Entities.Auth;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpDesk.Infrastructure.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly HelpDeskDbContext _context;
        public RolePermissionService(HelpDeskDbContext context) => _context = context;

        public async Task<bool> HasPermissionAsync(IEnumerable<string> roleNames, string permission, CancellationToken ct = default)
        {
            // Unir RolePermissions con Roles para filtrar por nombre de rol
            return await _context.RolePermissions
                .Join(_context.Roles,
                    rp => rp.RoleId,
                    r => r.Id,
                    (rp, r) => new { rp, r })
                .Where(x => roleNames.Contains(x.r.Name) && x.rp.Permission == permission)
                .AnyAsync(ct);
        }

        public async Task<IReadOnlyCollection<string>> GetPermissionsForRoleAsync(string roleName, CancellationToken ct = default)
        {
            return await _context.RolePermissions
                .Join(_context.Roles,
                    rp => rp.RoleId,
                    r => r.Id,
                    (rp, r) => new { rp, r })
                .Where(x => x.r.Name == roleName)
                .Select(x => x.rp.Permission)
                .ToListAsync(ct);
        }

        public async Task AddPermissionToRoleAsync(string roleName, string permission, CancellationToken ct = default)
        {
            // Verificar que el rol existe en Identity y obtener su ID
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
            if (role == null) throw new Exception($"Role '{roleName}' not found in Identity");

            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == role.Id && rp.Permission == permission, ct);
            if (exists) return; // ya existe

            var rp = new RolePermission { RoleId = role.Id, Permission = permission };
            _context.RolePermissions.Add(rp);
            await _context.SaveChangesAsync(ct);
        }

        public async Task RemovePermissionFromRoleAsync(string roleName, string permission, CancellationToken ct = default)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
            if (role == null) return; // Rol no encontrado, nada que eliminar

            var rp = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.Permission == permission, ct);
            if (rp == null) return;

            _context.RolePermissions.Remove(rp);
            await _context.SaveChangesAsync(ct);
        }
    }
}
