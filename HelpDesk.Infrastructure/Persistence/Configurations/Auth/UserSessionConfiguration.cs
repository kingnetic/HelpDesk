using HelpDesk.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpDesk.Infrastructure.Persistence.Configurations.Auth
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions", "security");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.JwtId)
                .IsRequired();

            builder.Property(x => x.TokenExpiresAt)
                .IsRequired();

            builder.Property(x => x.LoginAt)
                .IsRequired();

            builder.Property(x => x.IsActiveSession)
                .IsRequired();

            builder.Property(x => x.LoginIpAddress)
                .HasMaxLength(200);

            builder.Property(x => x.LoginUserAgent)
                .HasMaxLength(1000);

            builder.Property(x => x.LogoutIpAddress)
                .HasMaxLength(200);

            builder.Property(x => x.LogoutUserAgent)
                .HasMaxLength(1000);

            builder.Property(x => x.LogoutReason)
                .HasMaxLength(500);
        }
    }
}
