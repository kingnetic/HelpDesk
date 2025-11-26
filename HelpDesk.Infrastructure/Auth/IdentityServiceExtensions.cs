using HelpDesk.Infrastructure.Identity;
using HelpDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        })
        .AddRoles<Role>()
        .AddRoleManager<RoleManager<Role>>()
        .AddSignInManager<SignInManager<User>>()
        .AddUserManager<UserManager<User>>()
        .AddEntityFrameworkStores<HelpDeskDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, HelpDesk.Infrastructure.Auth.TicketOwnerAuthorizationHandler>();
        services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, HelpDesk.Infrastructure.Auth.TicketDtoAuthorizationHandler>();

        return services;
    }
}
