using TransitNovaPayment.Busieness.Common.Abstract.Abstraction;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods
{
    internal class CreditCard : PaymentMethodService
    {
        public override decimal Commision => 0.025m;

        public override PaymentMethod PaymentMethod => PaymentMethod.CreditCard;

        public override decimal Pay(decimal cost, Currency currency)
        {
            var rateExchange = GetCurrencyConversionRate(currency);
            return cost + (cost * Commision)/rateExchange;
        }
    }
}
