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
        public Guid ShipmentId { get; private set; }
        public Guid CustomerId { get; private set; }
        public UserProfile UserProfile { get;  set; } = null!;
        public decimal ShippingCost { get; private set; }
        public decimal Commission { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public string? Notes { get; private set; }

        public static PaymentInvoice Create(Guid paymentId, Guid shipmentId, Guid customerId, decimal shippingCost, decimal commission, decimal amount, PaymentMethod paymentMethod, PaymentStatus status, DateTime? paidAt = null, string? notes = null)
        {
            return new PaymentInvoice
            {
                Id = Guid.CreateVersion7(),
                PaymentId = paymentId,
                ShipmentId = shipmentId,
                CustomerId = customerId,
                ShippingCost = shippingCost,
                Commission = commission,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = status,
                PaidAt = paidAt,
                Notes = notes
            };
        }
    }
}
