using HelpDesk.Domain.Entities.HelpDesk;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations.HelpDesk
{
    public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
    {
        public void Configure(EntityTypeBuilder<TicketComment> builder)
        {
            builder.ToTable("TicketComments", "helpdesk");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Comment)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.TicketId).IsRequired();

            builder.HasOne<Ticket>()
                .WithMany(t => t.Comments)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
