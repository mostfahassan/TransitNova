using TransitNovaPayment.Busieness.Common.Abstract.Abstraction;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods
{
    internal class PaypalPayment : PaymentMethodService
    {
        public override decimal Commision => 0.045m;

        public override PaymentMethod PaymentMethod => PaymentMethod.PayPal;

        public override decimal Pay(decimal shippingCost, Currency currency)
        {
            var rateExchange = GetCurrencyConversionRate(currency);
            return shippingCost + (shippingCost * Commision) / rateExchange;
        }
    }
}
