using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Payment;

namespace TransitNova.Domain.Entities.MainEntities
{
    public sealed class PaymentInvoice : BaseEntity<Guid>
    {
        private PaymentInvoice()
        {
        }

        public Guid PaymentId { get; private set; }
        public Guid ReferecneId { get; private set; }
        public Guid CustomerId { get; private set; }
        public UserProfile UserProfile { get;  set; } = null!;
        public decimal Cost { get; private set; }
        public decimal Commission { get; private set; }
        public decimal Amount { get; private set; }
        public string ReferecneType { get; private set; } = null!;
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public string? Notes { get; private set; }
        public Guid? BundleSubscriptionId { get; private set; }
        public Guid? BundleId { get; private set; }
        public string? BundleName { get; private set; }
        public decimal OriginalShippingCost { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal FinalShippingCost { get; private set; }
        public bool SubscriptionBenefitApplied { get; private set; }

        public static PaymentInvoice Create(
            Guid paymentId,
            Guid referenceId,
            Guid customerId,
            decimal cost,
            decimal commission,
            decimal amount,
            PaymentMethod paymentMethod,
            PaymentStatus status,
            string referenceType,
            DateTime? paidAt = null,
            string? notes = null,
            Guid? bundleSubscriptionId = null,
            Guid? bundleId = null,
            string? bundleName = null,
            decimal? originalShippingCost = null,
            decimal discountPercentage = 0,
            decimal discountAmount = 0,
            decimal? finalShippingCost = null,
            bool subscriptionBenefitApplied = false)
        {
            var normalizedOriginalShippingCost = originalShippingCost ?? cost;
            var normalizedFinalShippingCost = finalShippingCost ?? cost;

            return new PaymentInvoice
            {
                Id = Guid.CreateVersion7(),
                PaymentId = paymentId,
                ReferecneId = referenceId,
                CustomerId = customerId,
                Cost = cost,
                Commission = commission,
                Amount = amount,
                PaymentMethod = paymentMethod,
                ReferecneType = referenceType,
                Status = status,
                PaidAt = paidAt,
                Notes = notes,
                BundleSubscriptionId = bundleSubscriptionId,
                BundleId = bundleId,
                BundleName = bundleName,
                OriginalShippingCost = normalizedOriginalShippingCost,
                DiscountPercentage = discountPercentage,
                DiscountAmount = discountAmount,
                FinalShippingCost = normalizedFinalShippingCost,
                SubscriptionBenefitApplied = subscriptionBenefitApplied
            };
        }
    }
}
