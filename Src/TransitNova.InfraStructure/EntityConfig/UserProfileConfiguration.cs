using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public partial class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> user)
        {
            user.HasKey(u => u.Id).IsClustered();

            user.HasMany(u => u.SentShipments)
            .WithOne(s => s.Sender)
            .HasForeignKey(s => s.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

            user.HasOne(u => u.City)
                .WithMany()
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

          
            user.HasOne<AppUser>()
                    .WithOne()
                    .HasForeignKey<UserProfile>(x => x.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);


            user.HasIndex(c => c.AppUserId).IsUnique();
            user.HasIndex(u => u.CityId);
            user.HasIndex(u => u.Email);
        }
   
    }
}
