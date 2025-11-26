using HelpDesk.Application.Interfaces;
using HelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HelpDesk.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ExistsAsync(int userId)
        {
            var u = await _userManager.FindByIdAsync(userId.ToString());
            return u != null;
        }

        public async Task<string> GetEmailByUserIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.Email ?? throw new Exception("User not found.");
        }

        public async Task<List<HelpDesk.Application.DTOs.UserDto>> GetAllUsersAsync(CancellationToken ct)
        {
            // Necesitamos importar EntityFrameworkCore para usar ToListAsync en IQueryable
            var users = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                System.Linq.Queryable.Select(_userManager.Users, u => new { u.Id, u.Email, u.FullName }),
                ct);

            var result = new List<HelpDesk.Application.DTOs.UserDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(new User { Id = u.Id }); // Esto puede ser lento (N+1), pero es aceptable para pocos usuarios
                result.Add(new HelpDesk.Application.DTOs.UserDto(u.Id, u.Email!, u.FullName ?? string.Empty, roles.ToList()));
            }
            return result;
        }
        public async Task<int> CreateUserAsync(string username, string email, string password, string fullName, List<string> roles)
        {
            var user = new User { UserName = username, Email = email, FullName = fullName };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (roles != null && roles.Any())
            {
                await _userManager.AddToRolesAsync(user, roles);
            }

            return user.Id;
        }

        public async Task<bool> UpdateUserAsync(int id, string email, string fullName, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            user.Email = email;
            user.UserName = email;
            user.FullName = fullName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var toAdd = roles.Except(currentRoles);
            var toRemove = currentRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, toAdd);
            await _userManager.RemoveFromRolesAsync(user, toRemove);

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
