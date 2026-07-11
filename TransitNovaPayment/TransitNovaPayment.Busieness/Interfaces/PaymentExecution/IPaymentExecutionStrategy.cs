namespace TransitNovaPayment.Busieness.Interfaces.PaymentExecution
{
    public interface IPaymentExecutionStrategy
    {
     Task<PaymentExecutionResult> ExecuteAsync(CancellationToken cancellationToken);
    }

    public sealed record PaymentExecutionResult(bool IsSuccess, string? FailureReason = null);
}
