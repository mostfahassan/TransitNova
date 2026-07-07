using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods
{
    internal class CreditCard : PaymentMethodService
    {
        public override decimal Commision => 0.025m;

        public override PaymentMethod PaymentMethod => PaymentMethod.CreditCard;

        public override decimal Pay(decimal shippingCost, Currency currency)
        {
            var rateExchange = GetCurrencyConversionRate(currency);
            return shippingCost + (shippingCost * Commision)/rateExchange;
        }
            
            


    }
}
