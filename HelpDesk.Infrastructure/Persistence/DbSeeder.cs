using HelpDesk.Domain.Entities.Auth;
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
            var roleManager = provider.GetRequiredService<RoleManager<HelpDesk.Infrastructure.Identity.Role>>();

            await context.Database.MigrateAsync();

            string[] roles = { HelpDesk.Domain.Constants.Roles.Admin, HelpDesk.Domain.Constants.Roles.Support, HelpDesk.Domain.Constants.Roles.Customer };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = new HelpDesk.Infrastructure.Identity.Role { Name = role };
                    var result = await roleManager.CreateAsync(r);

                    if (!result.Succeeded)
                        throw new Exception($"Error seeding role {role}: " +
                            string.Join(" | ", result.Errors.Select(e => e.Description)));
                }
            }

            // Inicializar permisos para el rol Admin
            await SeedPermissionsAsync(context);

            // Crear usuario Admin
            const string adminEmail = "admin@helpdesk.com";
            const string adminPassword = "Admin@123";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrador del Sistema",
                    EmailConfirmed = true
                };

                var create = await userManager.CreateAsync(admin, adminPassword);

                if (!create.Succeeded)
                    throw new Exception("No se pudo crear el usuario admin: " +
                        string.Join(" | ", create.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(admin, HelpDesk.Domain.Constants.Roles.Admin))
            {
                await userManager.AddToRoleAsync(admin, HelpDesk.Domain.Constants.Roles.Admin);
            }

            // Crear usuario Support
            const string supportEmail = "support@helpdesk.com";
            const string supportPassword = "Support@123";

            var support = await userManager.FindByEmailAsync(supportEmail);

            if (support == null)
            {
                support = new User
                {
                    UserName = supportEmail,
                    Email = supportEmail,
                    FullName = "Soporte Técnico",
                    EmailConfirmed = true
                };

                var createSupport = await userManager.CreateAsync(support, supportPassword);

                if (!createSupport.Succeeded)
                    throw new Exception("No se pudo crear el usuario support: " +
                        string.Join(" | ", createSupport.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(support, HelpDesk.Domain.Constants.Roles.Support))
            {
                await userManager.AddToRoleAsync(support, HelpDesk.Domain.Constants.Roles.Support);
            }

            // Inicializar catálogos recursivos
            await SeedCatalogsAsync(context);

        }

        private static async Task SeedCatalogsAsync(HelpDeskDbContext context)
        {
            if (await context.Catalogs.AnyAsync()) return;

            // Crear catálogos padre
            var statusCatalog = new Domain.Entities.Catalog.Catalog("Status", "Estado del ticket", 1);
            var priorityCatalog = new Domain.Entities.Catalog.Catalog("Priority", "Prioridad del ticket", 2);
            var categoryCatalog = new Domain.Entities.Catalog.Catalog("Category", "Categoría del ticket", 3);
            var typeCatalog = new Domain.Entities.Catalog.Catalog("Type", "Tipo de ticket", 4);

            context.Catalogs.AddRange(statusCatalog, priorityCatalog, categoryCatalog, typeCatalog);
            await context.SaveChangesAsync();

            // Crear valores para Status
            var statusValues = new[]
            {
                new Domain.Entities.Catalog.Catalog("Open", "open", statusCatalog.Id, "Ticket abierto", 1),
                new Domain.Entities.Catalog.Catalog("Assigned", "assigned", statusCatalog.Id, "Ticket asignado a un empleado", 2),
                new Domain.Entities.Catalog.Catalog("In Progress", "in_progress", statusCatalog.Id, "Ticket en progreso", 3),
                new Domain.Entities.Catalog.Catalog("Resolved", "resolved", statusCatalog.Id, "Ticket resuelto", 4),
                new Domain.Entities.Catalog.Catalog("Closed", "closed", statusCatalog.Id, "Ticket cerrado", 5)
            };

            // Crear valores para Priority
            var priorityValues = new[]
            {
                new Domain.Entities.Catalog.Catalog("Low", "low", priorityCatalog.Id, "Prioridad baja", 1),
                new Domain.Entities.Catalog.Catalog("Medium", "medium", priorityCatalog.Id, "Prioridad media", 2),
                new Domain.Entities.Catalog.Catalog("High", "high", priorityCatalog.Id, "Prioridad alta", 3),
                new Domain.Entities.Catalog.Catalog("Critical", "critical", priorityCatalog.Id, "Prioridad crítica", 4)
            };

            // Crear valores para Category
            var categoryValues = new[]
            {
                new Domain.Entities.Catalog.Catalog("Hardware", "hardware", categoryCatalog.Id, "Problemas de hardware", 1),
                new Domain.Entities.Catalog.Catalog("Software", "software", categoryCatalog.Id, "Problemas de software", 2),
                new Domain.Entities.Catalog.Catalog("Network", "network", categoryCatalog.Id, "Problemas de red", 3),
                new Domain.Entities.Catalog.Catalog("Access", "access", categoryCatalog.Id, "Problemas de acceso", 4),
                new Domain.Entities.Catalog.Catalog("Other", "other", categoryCatalog.Id, "Otros problemas", 5)
            };

            // Crear valores para Type
            var typeValues = new[]
            {
                new Domain.Entities.Catalog.Catalog("Incident", "incident", typeCatalog.Id, "Incidente", 1),
                new Domain.Entities.Catalog.Catalog("Request", "request", typeCatalog.Id, "Solicitud", 2),
                new Domain.Entities.Catalog.Catalog("Task", "task", typeCatalog.Id, "Tarea", 3),
                new Domain.Entities.Catalog.Catalog("Question", "question", typeCatalog.Id, "Pregunta", 4)
            };

            context.Catalogs.AddRange(statusValues);
            context.Catalogs.AddRange(priorityValues);
            context.Catalogs.AddRange(categoryValues);
            context.Catalogs.AddRange(typeValues);

            await context.SaveChangesAsync();
        }

        private static async Task SeedPermissionsAsync(HelpDeskDbContext context)
        {
            // Obtener los IDs de los roles
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == HelpDesk.Domain.Constants.Roles.Admin);
            var supportRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == HelpDesk.Domain.Constants.Roles.Support);

            if (adminRole == null || supportRole == null) return;

            // Lista de permisos para Admin
            var adminPermissions = new[]
            {
                "AssignTicket",
                "CloseTicket",
                "ResolveTicket",
                "DeleteTicket",
                "ManageRoles",
                "ManageUsers",
                "ViewAuditLogs",
                "ViewAllTickets"
            };

            foreach (var permission in adminPermissions)
            {
                var exists = await context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == adminRole.Id && rp.Permission == permission);

                if (!exists)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        Permission = permission
                    });
                }
            }

            // Permisos para Support
            var supportPermissions = new[]
            {
                "AssignTicket",
                "CloseTicket",
                "ResolveTicket",
                "ViewAllTickets"
            };

            foreach (var permission in supportPermissions)
            {
                var exists = await context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == supportRole.Id && rp.Permission == permission);

                if (!exists)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = supportRole.Id,
                        Permission = permission
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
