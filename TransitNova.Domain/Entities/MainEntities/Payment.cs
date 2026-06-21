
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Payment;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Payment :AggregateRoot<Guid>
    {
        public decimal Amount { get; private set; }
        public decimal Commission { get; private set; }
        public byte[] RowVersion { get; private set; } = default!; 
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentSatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public string? Notes { get; private set; }
        public Guid ShipmentId { get; private set; }
        public virtual Shipment Shipment { get; set; } = null!;
        public virtual ICollection<PaymentHistory> PaymentHistories { get; set; } = new List<PaymentHistory>();

        private Payment()
        {
        }


        private Payment(decimal amount, decimal commission, PaymentMethod paymentMethod, PaymentSatus status, string? notes, Guid shipmentId)
        {
            Id = Guid.CreateVersion7();
            Amount = amount;
            Commission = commission;
            PaymentMethod = paymentMethod;
            Status = status;
            Notes = notes;
            ShipmentId = shipmentId;
            CreatedAt = DateTime.UtcNow;
            CurrentState = true;
        }

        public static Payment Create(decimal amount, decimal commission, PaymentMethod paymentMethod, PaymentSatus status, string? notes, Guid shipmentId)
        {
            return new Payment(amount, commission, paymentMethod, status, notes, shipmentId);
        }
    }
}
