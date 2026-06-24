using TransitNovaPayment.Busieness.Models.Common;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Models.PaymentHistoryEntity
{
    public class PaymentHistory : BaseEntity<int>
    {
        public Guid PaymentId { get; private set; }
        public virtual Payment Payment { get; set; } = null!;
        public PaymentStatus OldStatus { get; private set; }
        public PaymentStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string? Reason { get; private set; }
    }
}
