using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class ShipmentStatusConfiguration : IEntityTypeConfiguration<ShipmentStatus>
    {
        public void Configure(EntityTypeBuilder<ShipmentStatus> shipmentStatus)
        {

            shipmentStatus.HasKey(ss => ss.Id);

            shipmentStatus.HasOne(ss => ss.Shipment)
                        .WithMany(s => s.ShipmentStates)
                        .HasForeignKey(ss => ss.ShipmentId)
                        .OnDelete(DeleteBehavior.Cascade);

            shipmentStatus.Property(x => x.CarrierId)
                        .HasColumnName("UserId");

            shipmentStatus.HasOne(ss => ss.Carrier)
                          .WithMany()
                          .HasForeignKey(ss => ss.CarrierId)
                          .OnDelete(DeleteBehavior.SetNull);

            shipmentStatus.Property(ss => ss.StatusType)
                .IsRequired();

            shipmentStatus.HasQueryFilter(ss => !ss.Shipment!.IsDeleted);

            shipmentStatus.HasIndex(ss => new { ss.ShipmentId, ss.CarrierId });

        }
    }


















}
