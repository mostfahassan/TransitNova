namespace TransitNovaPayment.Busieness.Common.Options
{
    public sealed class PaymentGatewaySettings
    {
        public const string SectionName = "PaymentSettings";

        public string? PrivateKey { get; init; }
    }
}
