using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods
{
    internal class WalletsPayment : PaymentMethodService
    {
        public override decimal Commision => 0.015m;
        public override PaymentMethod PaymentMethod => PaymentMethod.MobileWallets;
        public override decimal Pay(decimal shippingCost)
           => shippingCost + (shippingCost * Commision);
    }
}
