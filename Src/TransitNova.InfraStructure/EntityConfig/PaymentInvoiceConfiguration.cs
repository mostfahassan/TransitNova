using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.InfraStructure.EntityConfig
{
    internal class PaymentInvoiceConfiguration : IEntityTypeConfiguration<PaymentInvoice>
    {
        public void Configure(EntityTypeBuilder<PaymentInvoice> invoice)
        {
            invoice.HasKey(x => x.Id).IsClustered();

            invoice.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            invoice.Property(x => x.PaymentMethod)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            invoice.Property(x => x.Notes)
                   .HasMaxLength(1000);

            invoice.Property(x => x.BundleName)
                   .HasMaxLength(150);

            invoice.Property(x => x.Cost)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.Commission)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.Amount)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.OriginalShippingCost)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.DiscountPercentage)
                   .HasColumnType("decimal(5, 2)");

            invoice.Property(x => x.DiscountAmount)
                   .HasColumnType("decimal(18, 2)");

            invoice.Property(x => x.FinalShippingCost)
                   .HasColumnType("decimal(18, 2)");


            invoice.HasIndex(x => x.PaymentId);

            invoice.HasIndex(x => x.ReferecneId);

            invoice.HasIndex(x => x.CustomerId);

            invoice.HasIndex(x => x.CreatedAt);

            invoice.HasIndex(x => x.PaidAt);

            invoice.HasIndex(x => x.BundleId);

            invoice.HasIndex(x => x.BundleSubscriptionId);

            invoice.HasIndex(x => new { x.CustomerId, x.Status });
            invoice.HasIndex(x => new { x.ReferecneId, x.Status });
            invoice.HasIndex(x => new { x.CustomerId, x.BundleId, x.SubscriptionBenefitApplied, x.CreatedAt });

            invoice.HasOne(x => x.UserProfile)
                   .WithMany(x => x.PaymentInvoices)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
