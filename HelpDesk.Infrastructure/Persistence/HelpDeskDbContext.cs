using HelpDesk.Domain.Entities.Auth;
using HelpDesk.Domain.Entities.HelpDesk;
using HelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence
{
    public class HelpDeskDbContext
        : IdentityDbContext<User, HelpDesk.Infrastructure.Identity.Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
            : base(options)
        {
        }

        // Mesa de ayuda
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();
        public DbSet<TicketStatusHistory> StatusHistory => Set<TicketStatusHistory>();

        // Catálogo recursivo
        public DbSet<Domain.Entities.Catalog.Catalog> Catalogs => Set<Domain.Entities.Catalog.Catalog>();

        // Autenticación / Sesiones
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        // Auditoría
        public DbSet<TicketAuditLog> TicketAuditLogs => Set<TicketAuditLog>();
        public DbSet<Domain.Entities.Security.SecurityAuditLog> SecurityAuditLogs => Set<Domain.Entities.Security.SecurityAuditLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Esquema de Identity
            builder.HasDefaultSchema("security");

            // Aplicar configuraciones
            builder.ApplyConfigurationsFromAssembly(typeof(HelpDeskDbContext).Assembly);
        }
    }
}
