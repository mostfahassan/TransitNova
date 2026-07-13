using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> vehicle)
        {
            vehicle.HasKey(v => v.Id);

            vehicle.HasOne(x => x.Carrier)
                .WithOne(c => c.Vehicle)
                .HasForeignKey<Vehicle>(x => x.CarrierId)
                .OnDelete(DeleteBehavior.Restrict);
            vehicle.Property(x => x.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            vehicle.Property(w => w.RowVersion)
                .IsRowVersion();


            vehicle.Property(x => x.CapacityWeight)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            vehicle.Property(x => x.CapacityVolume)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            vehicle.Property(x => x.VehicleType)
                .IsRequired()
                .HasConversion<string>();

            vehicle.Property(x => x.IsRefrigerated)
                .IsRequired()
                .HasDefaultValue(false);

            vehicle.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            vehicle.Property(x => x.VehicleType)
                .HasConversion<string>();

            vehicle.HasQueryFilter(v =>
            !v.Carrier.IsDeleted &&
             v.IsActive);

            vehicle.HasIndex(v => v.PlateNumber).IsUnique();
            vehicle.HasIndex(v => v.CarrierId);
            vehicle.HasIndex(v => v.IsActive);
            vehicle.HasIndex(v => v.VehicleType);
            vehicle.HasIndex(v => new { v.CarrierId, v.IsActive });
            vehicle.HasIndex(v => v.IsRefrigerated);
        }
    }


















}
