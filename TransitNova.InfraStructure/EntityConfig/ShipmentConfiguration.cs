using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> shipment)
        {
            shipment.HasQueryFilter(sh => !sh.IsDeleted);


            shipment.Property(sh => sh.RowVersion)
                .IsRowVersion();
            shipment.HasOne(s => s.HandledBy)
                    .WithMany(op => op.HandledShipments)
                    .HasForeignKey(s => s.HandledById)
                    .OnDelete(DeleteBehavior.SetNull);

            shipment.Property(s => s.ShipmentCost)
                    .HasPrecision(18, 2);
           

            shipment.HasQueryFilter(SH => !SH.IsDeleted);

            shipment.OwnsOne(x => x.PackageSpecification, nav =>
             {
                 nav.Property(p => p.Weight).HasColumnName("Weight");
                 nav.Property(p => p.Width).HasColumnName("Width");
                 nav.Property(p => p.Height).HasColumnName("Height");
                 nav.Property(p => p.Length).HasColumnName("Length");
             });

            shipment.HasIndex(s => s.TrackingNumber).IsUnique();
            shipment.HasIndex(s => s.CurrentStatus);
            shipment.HasIndex(s => s.SenderId);
            shipment.HasIndex(s => s.ReceiverId);
            shipment.HasIndex(s => s.PickupDate);
            shipment.HasIndex(s => s.TripId);
            shipment.HasIndex(s => new { s.CurrentStatus, s.PickupDate });
            shipment.HasIndex(s => new { s.SenderId, s.CurrentStatus });
            shipment.HasIndex(s => s.HandledById);
            shipment.HasIndex(s => s.ActualDeliveryDate);
            shipment.HasIndex(s => s.CreatedAt);
            shipment.HasIndex(s => s.UpdatedAt);
            shipment.HasIndex(s => new { s.CurrentStatus, s.PickupDate, s.CreatedAt });
            shipment.HasIndex(s => new { s.SenderId, s.CurrentStatus, s.ActualDeliveryDate });
        }
    }
}
