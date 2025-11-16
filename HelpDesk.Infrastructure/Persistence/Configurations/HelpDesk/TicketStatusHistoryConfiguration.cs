using HelpDesk.Domain.Entities.HelpDesk;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations.HelpDesk
{
    public class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
    {
        public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
        {
            builder.ToTable("TicketStatusHistory", "helpdesk");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StatusId).IsRequired();

            builder.Property(x => x.OccurredAt)
                .IsRequired();

            // RELACIÓN CORRECTA
            builder.HasOne(x => x.Ticket)
                .WithMany(t => t.StatusHistory)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
