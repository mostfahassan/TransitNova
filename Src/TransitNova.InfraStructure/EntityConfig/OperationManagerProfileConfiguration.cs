using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class OperationManagerProfileConfiguration : IEntityTypeConfiguration<OperationManagerProfile>
    {
        public void Configure(EntityTypeBuilder<OperationManagerProfile> admin)
        {
            admin.HasKey(a => a.Id)
                 .IsClustered();

          
            admin.HasOne(c => c.City)
                .WithMany()
                .HasForeignKey(c => c.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            admin.HasIndex(c => c.AppUserId).IsUnique();
            admin.HasOne<AppUser>()
                    .WithOne()
                    .HasForeignKey<OperationManagerProfile>(x => x.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
