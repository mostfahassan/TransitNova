using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TransitNova.BusinessLayer.Options;

namespace TransitNova.InfraStructure.Health
{
    internal sealed class PaymentGatewayConfigurationHealthCheck(IOptions<PaymentSettings> paymentOptions) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var settings = paymentOptions.Value;
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.PublicKey))
                failures.Add("PaymentSettings:PublicKey is missing.");

            if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var uri) ||
                uri.Scheme is not ("http" or "https"))
            {
                failures.Add("PaymentSettings:BaseUrl must be an absolute HTTP(S) URL.");
            }

            return Task.FromResult(failures.Count == 0
                ? HealthCheckResult.Healthy("Payment gateway configuration is valid.")
                : HealthCheckResult.Unhealthy(string.Join(' ', failures)));
        }
    }
}
