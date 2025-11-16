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
    }
}
