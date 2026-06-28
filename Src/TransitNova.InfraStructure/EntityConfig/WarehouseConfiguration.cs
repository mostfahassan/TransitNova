using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> wareHouse)
        {

            wareHouse.HasKey(w => w.Id).IsClustered();

            wareHouse.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);

            wareHouse.Property(w => w.Address)
                .IsRequired()
                .HasMaxLength(200);

            wareHouse.Property(w => w.Capacity)
                .HasPrecision(18, 2);

            wareHouse.Property(w => w.RowVersion)
                .IsRowVersion();

            wareHouse.Property(w => w.CurrentUsage)
                .HasPrecision(18, 2);

            wareHouse.HasMany(w => w.ZonesServed)
            .WithMany(z => z.Warehouses)
            .UsingEntity<Dictionary<string, object>>(
                "WarehouseZone",
                right => right.HasOne<Zone>()
                    .WithMany()
                    .HasForeignKey("ZoneId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Warehouse>()
                    .WithMany()
                    .HasForeignKey("WarehouseId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("WarehouseZones");
                    join.HasKey("WarehouseId", "ZoneId");
                });

            
            wareHouse.HasIndex(w => w.CreatedBy);
            wareHouse.HasIndex(w => w.Name);
            wareHouse.HasIndex(w => w.Type);
            wareHouse.HasIndex(w => w.ManagerId);
            

        }
    }
}
