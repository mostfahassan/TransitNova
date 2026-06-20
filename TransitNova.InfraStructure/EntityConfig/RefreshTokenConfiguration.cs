using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> token)
        {
            token.HasKey(t => t.Id);

            token.Property(t => t.Token)
                .HasMaxLength(150);

            token.HasIndex(t => t.Token)
                .IsUnique();

            token.HasOne(t => t.User)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            token.HasIndex(t => t.UserId);
            token.HasIndex(t => t.ExpiresAt);
            token.HasIndex(t => new { t.UserId, t.ExpiresAt });
            // Add filter if revocation is tracked
            token.HasQueryFilter(t => !t.IsRevoked);
        }
    }


















}
