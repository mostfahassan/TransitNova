using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Models.PaymentHistoryEntity
{
    public class PaymentHistory : BaseEntity<int>
    {
        public Guid PaymentId { get;  set; }
        public virtual Payment Payment { get; set; } = null!;
        public PaymentStatus OldStatus { get;  set; }
        public PaymentStatus NewStatus { get; set; }
        public DateTime ChangedAt { get;  set; }
    }
}
