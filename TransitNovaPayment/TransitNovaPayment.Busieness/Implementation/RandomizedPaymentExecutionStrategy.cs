using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.Busieness.Interfaces.PaymentExecution;

namespace TransitNovaPayment.Busieness.Implementation
{
    internal sealed class RandomizedPaymentExecutionStrategy(
        IOptions<PaymentExecutionOptions> options) : IPaymentExecutionStrategy
    {
        private static readonly string[] FailureReasons =
        [
            "Insufficient funds in the account.",
            "Card has expired or invalid details provided.",
            "Payment gateway connection timeout.",
            "Transaction declined by the issuing bank."
        ];

        public async Task<PaymentExecutionResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            var settings = options.Value;
            if (settings.DelayMilliseconds > 0)
                await Task.Delay(TimeSpan.FromMilliseconds(settings.DelayMilliseconds), cancellationToken);

            if (settings.ForcedSuccess.HasValue)
            {
                return settings.ForcedSuccess.Value
                    ? new PaymentExecutionResult(true)
                    : new PaymentExecutionResult(false, FailureReasons[0]);
            }

            var isSuccess = Random.Shared.Next(0, 10) > 2;
            return isSuccess
                ? new PaymentExecutionResult(true)
                : new PaymentExecutionResult(false, FailureReasons[Random.Shared.Next(FailureReasons.Length)]);
        }
    }
}
