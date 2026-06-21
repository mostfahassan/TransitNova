using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public sealed class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
    {
        public void Configure(EntityTypeBuilder<Carrier> carrier)
        {
            carrier.HasKey(c => c.Id);

            carrier.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            carrier.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            carrier.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(20);

            carrier.Property(c => c.PhoneNumber)
                .HasMaxLength(15)
                .IsRequired();

            carrier.Property(c => c.Email)
                .HasMaxLength(50)
                .IsRequired();

            carrier.Property(c => c.Address)
                .HasMaxLength(100)
                .IsRequired();

            carrier.Property(c => c.MaxDailyShipments)
                .IsRequired();
            carrier.Property(c => c.RowVersion)
                .IsRowVersion();

            carrier.HasIndex(c => c.Code).IsUnique();
            carrier.HasIndex(c => c.AppUserId).IsUnique();
            carrier.HasIndex(c => c.Status);
            carrier.HasIndex(c => new { c.Status, c.AverageRating });
            carrier.HasIndex(c => c.Email);
            carrier.HasIndex(c => c.AverageRating);
            carrier.HasIndex(c => c.HandlerId);
            carrier.HasIndex(c => c.CityId);
            carrier.HasIndex(c => c.UpdatedAt);


            carrier.HasOne(c => c.City)
                .WithMany()
                .HasForeignKey(c => c.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            carrier.HasOne(c => c.HandledBy)
                .WithMany(op => op.HandledCarriers)
                .HasForeignKey(c => c.HandlerId)
                .OnDelete(DeleteBehavior.SetNull);

            carrier.HasMany(c => c.ServedZones)
                .WithMany(z => z.ServedByCarriers)
                .UsingEntity<Dictionary<string, object>>(
                    "CarrierZone",
                    right => right.HasOne<Zone>()
                        .WithMany()
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Carrier>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("CarrierZones");
                        join.HasKey("UserId", "ZoneId");
                    });

            carrier.HasQueryFilter(c => !c.IsDeleted && c.HasAdditionalInfo);
            carrier.Property(c => c.DefaultCostPerKg)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasPrecision(18, 2);



            carrier.HasOne(C => C.HomeWarehouse)
                .WithMany()
                .HasForeignKey(c => c.HomeWarehouseId)
                .OnDelete(DeleteBehavior.SetNull);


            carrier.HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<Carrier>(x => x.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}