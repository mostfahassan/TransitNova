namespace TransitNovaPayment.Busieness.Common.Options;

public sealed class PaymentExecutionOptions
{
    public const string SectionName = "PaymentExecution";

    public int DelayMilliseconds { get; init; } = 5000;

    public bool? ForcedSuccess { get; init; }
}
