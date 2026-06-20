using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.InfraStructure.EntityConfig
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> payment)
        {
            payment.HasKey(p => p.Id);


            payment.Property(p => p.PaymentMethod)
                .IsRequired();
            payment.Property(p => p.RowVersion)
                .IsRowVersion();

            payment.HasOne(p => p.Shipment)
                .WithOne(s => s.Payment)
                .HasForeignKey<Payment>(p => p.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            payment.HasIndex(p => p.ShipmentId).IsUnique();
            payment.HasIndex(p => p.PaymentMethod); 
            payment.HasIndex(p => p.Status);
            payment.HasIndex(p => p.CreatedAt);
            payment.HasIndex(p => p.UpdatedAt);
            payment.HasIndex(p => new { p.Status, p.CreatedAt });
            payment.HasIndex(p => new { p.PaymentMethod, p.Status });

            payment.HasQueryFilter(p => !p.Shipment!.IsDeleted);

        }
    }


}
