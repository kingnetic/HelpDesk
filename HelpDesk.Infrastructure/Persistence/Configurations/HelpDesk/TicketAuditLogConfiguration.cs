using HelpDesk.Domain.Entities.HelpDesk;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations.HelpDesk
{
    public class TicketAuditLogConfiguration : IEntityTypeConfiguration<TicketAuditLog>
    {
        public void Configure(EntityTypeBuilder<TicketAuditLog> builder)
        {
            builder.ToTable("TicketAuditLogs", "helpdesk");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Detail)
                .HasColumnType("text");

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(500);
        }
    }
}
