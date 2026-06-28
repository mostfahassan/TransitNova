using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public class WarehouseManagerProfileConfiguration : IEntityTypeConfiguration<WarehouseManagerProfile>
    {
        public void Configure(EntityTypeBuilder<WarehouseManagerProfile> manager)
        {
            manager.HasKey(m => m.Id).IsClustered();
            manager.HasOne(u => u.City)
                .WithMany()
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            manager.HasIndex(c => c.AppUserId).IsUnique();

            manager.HasOne<AppUser>()
                    .WithOne()
                    .HasForeignKey<UserProfile>(x => x.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);

            manager.HasIndex(u => u.CityId);
            manager.HasIndex(u => u.AppUserId).IsUnique();
            manager.HasIndex(u => u.Email);
            manager.HasOne(m => m.Warehouse)
                .WithOne(w => w.Manager)
                .HasForeignKey<Warehouse>(w => w.ManagerId);
        }
   
    }
}
