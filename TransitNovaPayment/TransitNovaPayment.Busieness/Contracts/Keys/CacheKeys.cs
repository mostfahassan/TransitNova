using System.Text.Json;
namespace TransitNovaPayment.Busieness.Contracts.Keys
{
    public static class CacheKeys
    {
        public static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(20);

        public const string PaymentsPrefix = "payments";

        public static string PaymentHistoryFilter(object filter)
            => $"{PaymentsPrefix}:history:filter:{Serialize(filter)}";

        private static string Serialize(object? value)
            => Uri.EscapeDataString(JsonSerializer.Serialize(value));
    }
}
