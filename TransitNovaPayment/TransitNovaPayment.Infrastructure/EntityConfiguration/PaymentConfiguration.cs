
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
namespace TransitNovaPayment.Infrastructure.EntityConfiguration
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

            payment.HasMany(p => p.Histories)
                .WithOne(h => h.Payment)
                .HasForeignKey(p => p.PaymentId)
                .OnDelete(DeleteBehavior.Restrict); 
 
            payment.HasIndex(p => p.ShipmentId).IsUnique();
            payment.HasIndex(p => p.PaymentMethod);
            payment.HasIndex(p => p.Status);
            payment.HasIndex(p => p.CreatedAt);
            payment.HasIndex(p => p.UpdatedAt);
            payment.HasIndex(p => new { p.Status, p.CreatedAt });
            payment.HasIndex(p => new { p.PaymentMethod, p.Status });
        }
    }
}
