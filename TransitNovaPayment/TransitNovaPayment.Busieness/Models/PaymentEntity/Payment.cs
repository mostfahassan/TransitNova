using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using TransitNovaPayment.Busieness.Models.PaymentHistoryEntity;
namespace TransitNovaPayment.Busieness.Models.PaymentEntity
{
    public  class Payment : BaseEntity<Guid>
    {
        private readonly List<PaymentHistory> PaymentHistories = [] ;
        public decimal TotalAmount { get; private set; }
        public byte[] RowVersion { get; private set; } = default!;
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        public string? FailedReason { get; private set; }
        public Guid ReferenceId { get; private set; }
        public ReferenceType ReferenceType { get; private set; }
        public IReadOnlyCollection<PaymentHistory> Histories => PaymentHistories;      

        private Payment()
        {
        }

        private Payment(decimal amount, Guid shipmentId, PaymentMethod paymentMethod, ReferenceType referenceType)
        {
            Id = Guid.CreateVersion7();
            TotalAmount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            ReferenceId = shipmentId;
            ReferenceType = referenceType;
        }

        public static Payment Create(decimal amount, Guid shipmentId,  PaymentMethod paymentMethod, ReferenceType referenceType)
        {
            return new Payment(amount, shipmentId, paymentMethod, referenceType);
        }

        PaymentHistory AddPaymentHistory(PaymentStatus status)
            => new()
            {
                PaymentId = Id,
                OldStatus = PaymentStatus.Pending,
                NewStatus = status,
                ChangedAt = DateTime.UtcNow,
            };

        public void MarkAsSucess()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException();

            Status = PaymentStatus.Success;
            PaidAt = DateTime.UtcNow;
            PaymentHistories.Add(AddPaymentHistory(PaymentStatus.Success));
        }
        public void MarkAsFailure(string reason)
        {
            Status = PaymentStatus.Failed;
            FailedReason = reason;
            FailedAt = DateTime.UtcNow;
            PaymentHistories.Add(AddPaymentHistory(PaymentStatus.Failed));
        }
    }
}
