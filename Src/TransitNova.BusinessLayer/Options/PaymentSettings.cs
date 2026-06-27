namespace TransitNova.BusinessLayer.Options
{
    public sealed class PaymentSettings
    {
        public const string SectionName = "PaymentSettings";

        public string? PublicKey { get; init; }
        public string? BaseUrl { get; init; }
    }
}
