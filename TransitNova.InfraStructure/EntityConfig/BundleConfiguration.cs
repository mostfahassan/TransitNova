using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public sealed class BundleConfiguration : IEntityTypeConfiguration<Bundle>
    {
        public void Configure(EntityTypeBuilder<Bundle> bundle)
        {

            bundle.HasKey(b => b.Id).IsClustered();
            bundle.Property(b => b.BundleName)
                  .IsRequired()
                  .HasMaxLength(50);
            bundle.Property(b => b.TotalShipments)
                  .IsRequired();

            bundle.Property(b => b.TotalWeight)
                  .IsRequired();

            bundle.Property(b => b.CurrentState)
                  .IsRequired();

            bundle.HasMany(b => b.Shipments)
                  .WithOne(s => s.PackageBundle)
                  .HasForeignKey(s => s.PackageBundleId)
                  .OnDelete(DeleteBehavior.SetNull); 

            bundle.Property(b => b.BundlePrice)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            bundle.HasIndex(b => b.BundleName).IsUnique();

            bundle.HasIndex(b => b.CurrentState);
            bundle.HasIndex(b => b.CreatedAt);
            bundle.HasIndex(b => b.UpdatedAt);
            bundle.HasIndex(b => new { b.CurrentState, b.CreatedAt });
            bundle.HasIndex(b => new { b.BundlePrice, b.CurrentState });

        }
    }


}
