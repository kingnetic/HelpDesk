using HelpDesk.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations
{
    public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder.ToTable("Catalogs", "catalog");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Value)
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // Configuración de relación recursiva
            builder.HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar eliminación en cascada

            // Índices
            builder.HasIndex(c => c.ParentId);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => new { c.ParentId, c.DisplayOrder });
        }
    }
}
