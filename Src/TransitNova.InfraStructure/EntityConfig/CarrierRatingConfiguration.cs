using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class CarrierRatingConfiguration : IEntityTypeConfiguration<CarrierRating>
    {
        public void Configure(EntityTypeBuilder<CarrierRating> carrierRating)
        {
            carrierRating.HasKey(c => c.Id);
            carrierRating.Property(c => c.Comment)
                .HasMaxLength(250);

            carrierRating.HasIndex(c => new { c.CarrierId, c.ShipmentId });
            carrierRating.HasIndex(c => c.CarrierId);
            carrierRating.HasIndex(c => c.ShipmentId).IsUnique();

        }
    }
}