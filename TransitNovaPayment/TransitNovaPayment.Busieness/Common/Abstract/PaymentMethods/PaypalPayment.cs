using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods
{
    internal class PaypalPayment : PaymentMethodService
    {
        public override decimal Commision => 0.045m;

        public override PaymentMethod PaymentMethod => PaymentMethod.CreditCard;

        public override decimal Pay(decimal shippingCost)
        => shippingCost + (shippingCost * Commision);
    }
}
