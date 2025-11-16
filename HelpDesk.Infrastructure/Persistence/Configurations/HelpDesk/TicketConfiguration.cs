using HelpDesk.Domain.Entities.HelpDesk;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations.HelpDesk
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets", "helpdesk");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Description)
                .HasColumnType("text");

            builder.Property(x => x.StatusId).IsRequired();
            builder.Property(x => x.PriorityId).IsRequired();
            builder.Property(x => x.CategoryId).IsRequired();
            builder.Property(x => x.TypeId).IsRequired(false);
            builder.Property(x => x.CreatedById).IsRequired();
            builder.Property(x => x.AssignedToEmployeeId).IsRequired(false);


            builder
                .HasMany(t => t.Comments)
                .WithOne()
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Navigation(t => t.Comments)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(t => t.StatusHistory)
                .WithOne()
                .HasForeignKey(h => h.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(t => t.StatusHistory)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
