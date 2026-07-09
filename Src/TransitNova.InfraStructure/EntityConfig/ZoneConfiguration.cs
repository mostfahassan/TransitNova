using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> zone)
        {

            zone.HasKey(z => z.Id);

            zone.HasOne(z => z.City)
                .WithMany(c => c.Zones)
                .HasForeignKey(z => z.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            zone.HasMany(z => z.Warehouses)
                .WithMany(w => w.ZonesServed);

            zone.HasIndex(z => z.CityId);
            zone.HasIndex(z => z.Name);

        }
    }

 }