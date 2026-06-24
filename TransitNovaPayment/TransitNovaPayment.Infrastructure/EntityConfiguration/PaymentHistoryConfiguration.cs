using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransitNovaPayment.Busieness.Models.PaymentHistoryEntity;

namespace TransitNovaPayment.Infrastructure.EntityConfiguration
{
    public class PaymentHistoryConfiguration : IEntityTypeConfiguration<PaymentHistory>
    {
        public void Configure(EntityTypeBuilder<PaymentHistory> paymentHistory)
        {
            paymentHistory.HasOne(ph => ph.Payment)
            .WithMany(p => p.PaymentHistories)
            .HasForeignKey(ph => ph.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

            paymentHistory.HasIndex(p => p.PaymentId);
            paymentHistory.HasIndex(p => p.CreatedAt);
            paymentHistory.HasIndex(p => p.NewStatus);
            paymentHistory.HasIndex(p => p.OldStatus);
            paymentHistory.HasIndex(p => new { p.PaymentId, p.CreatedAt });
        }
    }
}
