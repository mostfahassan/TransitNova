using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentExecution;

namespace TransitNovaPayment.Busieness.Common.Implementation
{
    internal sealed class RandomizedPaymentExecutionStrategy : IPaymentExecutionStrategy
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
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            var isSuccess = Random.Shared.Next(0, 10) > 2;
            return isSuccess
                ? new PaymentExecutionResult(true)
                : new PaymentExecutionResult(false, FailureReasons[Random.Shared.Next(FailureReasons.Length)]);
        }
    }
}
