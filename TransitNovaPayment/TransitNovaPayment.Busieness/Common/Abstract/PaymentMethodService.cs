using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract
{
    public abstract class PaymentMethodService
    {
        public abstract decimal Commision {  get; }
        public abstract PaymentMethod PaymentMethod {  get; }
        public abstract decimal Pay(decimal shippingCost);
    }
}
