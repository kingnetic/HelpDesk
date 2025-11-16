using HelpDesk.Domain.Entities.Auth;
using HelpDesk.Domain.Entities.HelpDesk;
using HelpDesk.Domain.ValueObjects;
using HelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence
{
    public class HelpDeskDbContext
        : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options)
            : base(options)
        {
        }

        // Helpdesk
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();
        public DbSet<TicketStatusHistory> StatusHistory => Set<TicketStatusHistory>();
        public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

        // Auth / Sessions
        public DbSet<UserSession> UserSessions => Set<UserSession>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity schema
            builder.HasDefaultSchema("security");

            // Apply configurations
            builder.ApplyConfigurationsFromAssembly(typeof(HelpDeskDbContext).Assembly);
        }
    }
}
