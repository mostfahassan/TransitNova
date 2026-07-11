using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract.Abstraction
{
    public abstract class PaymentMethodService
    {
        public abstract decimal Commision {  get; }
        public abstract PaymentMethod PaymentMethod {  get; }
        public abstract decimal Pay(decimal cost, Currency currency);

        protected decimal GetCurrencyConversionRate(Currency currency)
        {
            return currency switch
            {
                Currency.USD => 50m,
                Currency.EUR => 58m,
                Currency.EGB => 1.0m,
                _ => throw new NotSupportedException($"Currency {currency} is not supported.")
            };
        }
    }
}
