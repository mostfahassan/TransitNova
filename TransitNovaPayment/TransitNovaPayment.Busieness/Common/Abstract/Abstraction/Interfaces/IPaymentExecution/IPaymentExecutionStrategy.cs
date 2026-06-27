namespace TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentExecution
{
    public interface IPaymentExecutionStrategy
    {
        Task<PaymentExecutionResult> ExecuteAsync(CancellationToken cancellationToken);
    }

    public sealed record PaymentExecutionResult(bool IsSuccess, string? FailureReason = null);
}
