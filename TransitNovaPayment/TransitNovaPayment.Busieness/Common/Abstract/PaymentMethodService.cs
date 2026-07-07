using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
namespace TransitNovaPayment.Busieness.Common.Abstract
{
    public abstract class PaymentMethodService
    {
        public abstract decimal Commision {  get; }
        public abstract PaymentMethod PaymentMethod {  get; }
        public abstract decimal Pay(decimal shippingCost, Currency currency);

        protected decimal GetCurrencyConversionRate(Currency currency)
        {
            // Implement your logic to get the currency conversion rate based on the provided currency.
            // This is a placeholder implementation; you should replace it with actual logic.
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
