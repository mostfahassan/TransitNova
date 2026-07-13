using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> trip)
        {

            trip.HasKey(t => t.Id);

            trip.Property(t => t.RowVersion)
                .IsRowVersion();

            trip.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(10);

            trip.Property(t => t.TripType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(10);


            trip.HasOne(t => t.Carrier)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.CarrierId)
                .OnDelete(DeleteBehavior.Cascade);

            trip.HasOne(t => t.Warehouse)
                .WithMany(w => w.Trips)
                .HasForeignKey(t => t.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            trip.HasMany(t => t.Shipments)
                .WithOne(s => s.Trip)
                .HasForeignKey(s => s.TripId)
                .OnDelete(DeleteBehavior.SetNull);

            trip.HasQueryFilter(t => !t.Carrier!.IsDeleted);

            trip.HasIndex(t => t.WarehouseId);
            trip.HasIndex(t => t.Status);
            trip.HasIndex(t => t.CarrierId);
            trip.HasIndex(t => t.CreatedAt);
            trip.HasIndex(t => new { t.Status, t.CreatedAt });
            trip.HasIndex(t => new { t.CarrierId, t.Status });
            trip.HasIndex(t => new { t.WarehouseId, t.Status });



        }
    }
}
