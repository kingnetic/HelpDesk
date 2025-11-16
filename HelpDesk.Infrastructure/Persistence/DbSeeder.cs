using HelpDesk.Domain.ValueObjects;
using HelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDesk.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<HelpDeskDbContext>();
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var roleManager = provider.GetRequiredService<RoleManager<Role>>();

            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "Support", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = new Role { Name = role };
                    var result = await roleManager.CreateAsync(r);

                    if (!result.Succeeded)
                        throw new Exception($"Error seeding role {role}: " +
                            string.Join(" | ", result.Errors.Select(e => e.Description)));
                }
            }

            const string adminEmail = "marangoes@gmail.com";
            const string adminPassword = "Admin123#";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Marvin Gonzalez",
                    EmailConfirmed = true
                };

                var create = await userManager.CreateAsync(admin, adminPassword);

                if (!create.Succeeded)
                    throw new Exception("Unable to create admin user: " +
                        string.Join(" | ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (!await context.CatalogItems.AnyAsync())
            {
                context.CatalogItems.AddRange(
                    new CatalogItem("Open", "Status"),
                    new CatalogItem("Assigned", "Status"),
                    new CatalogItem("Resolved", "Status"),
                    new CatalogItem("Closed", "Status"),

                    new CatalogItem("Low", "Priority"),
                    new CatalogItem("Medium", "Priority"),
                    new CatalogItem("High", "Priority"),

                    new CatalogItem("Hardware", "Category"),
                    new CatalogItem("Software", "Category"),
                    new CatalogItem("Network", "Category"),

                    new CatalogItem("Incident", "Type"),
                    new CatalogItem("Request", "Type"),
                    new CatalogItem("Task", "Type"),
                    new CatalogItem("Question", "Type")

                );

                await context.SaveChangesAsync();
            }

        }
    }
}
