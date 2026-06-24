using TransitNovaPayment.Busieness.Models.Common;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using TransitNovaPayment.Busieness.Models.PaymentHistoryEntity;
namespace TransitNovaPayment.Busieness.Models.PaymentEntity
{
    public  class Payment : BaseEntity<Guid>
    {
        public decimal Amount { get; private set; }
        public decimal Commission { get; private set; }
        public byte[] RowVersion { get; private set; } = default!;
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public string? Notes { get; private set; }
        public Guid ShipmentId { get; private set; }
      
        public virtual ICollection<PaymentHistory> PaymentHistories { get; set; } = new List<PaymentHistory>();

        private Payment()
        {
        }

        private Payment(decimal amount, PaymentMethod paymentMethod, string? notes, Guid shipmentId)
        {
            Id = Guid.CreateVersion7();
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            Notes = notes;
            ShipmentId = shipmentId;
            CurrentState = true;
        }

        public static Payment Create(decimal amount,  PaymentMethod paymentMethod, PaymentStatus status, string? notes, Guid shipmentId)
        {
            return new Payment(amount, paymentMethod, notes, shipmentId);
        }
    }
}
