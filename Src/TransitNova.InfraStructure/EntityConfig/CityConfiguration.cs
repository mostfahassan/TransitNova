using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> city)
        {

            city.HasKey(c => c.Id);

            city.HasOne(c => c.Government)
                .WithMany(g => g.Cities)
                .HasForeignKey(c => c.GovernmentId);

            city.HasMany(c => c.Zones)
                .WithOne(z => z.City)
                .HasForeignKey(z => z.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            city.HasIndex(c => c.GovernmentId);
            city.HasIndex(c => c.Name);
            city.HasIndex(c => new { c.GovernmentId, c.Name }).IsUnique();
        }
    }
}
