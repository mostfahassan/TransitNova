using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    public sealed class BundleConfiguration : IEntityTypeConfiguration<Bundle>
    {
        public void Configure(EntityTypeBuilder<Bundle> bundle)
        {

            bundle.ToTable("Bundles");

            bundle.HasKey(b => b.Id);

            bundle.Property(b => b.BundleName)
                .IsRequired()
                .HasMaxLength(150);

            bundle.Property(b => b.BundleDescription)
                .HasMaxLength(500);

        
            bundle.Property(b => b.BundlePrice)
                .HasColumnType("decimal(18,2)");

            bundle.Property(b => b.MaxWeightPerShipment)
                .HasColumnType("decimal(18,2)");

            bundle.Property(b => b.MaxDistancePerShipment)
                .HasColumnType("decimal(18,2)");

            bundle.Property(b => b.MinimumShipmentValueForDiscount)
                .HasColumnType("decimal(18,2)");

         
            bundle.Property(b => b.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

         
            bundle.Property(b => b.Tier)
                .HasConversion<string>()
                .HasMaxLength(20);

           
            bundle.Metadata
                .FindNavigation(nameof(Bundle.Subscriptions))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

          
            bundle.HasMany(b => b.Subscriptions)
                .WithOne() 
                .HasForeignKey(s => s.BundleId)
                .OnDelete(DeleteBehavior.Restrict);

            bundle.Metadata
                .FindNavigation(nameof(Bundle.Subscriptions))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);



            bundle.HasIndex(b => b.BundleName).IsUnique();
            bundle.HasIndex(b => b.CurrentState);
            bundle.HasIndex(b => b.CreatedAt);
            bundle.HasIndex(b => b.UpdatedAt);
            bundle.HasIndex(b => new { b.CurrentState, b.CreatedAt });
            bundle.HasIndex(b => new { b.BundlePrice, b.CurrentState });
        }
    }


}
