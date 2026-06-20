
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Payment;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class PaymentHistory : BaseEntity<int>
    {
        public Guid PaymentId { get; private set; }
        public virtual Payment Payment { get; set; } = null!;
        public PaymentSatus OldStatus { get; private set; }
        public PaymentSatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string? Reason { get; private set; }
    }
}

