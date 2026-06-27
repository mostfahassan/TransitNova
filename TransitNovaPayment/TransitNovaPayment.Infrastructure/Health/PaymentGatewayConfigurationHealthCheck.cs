using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Options;

namespace TransitNovaPayment.InfraStructure.Health
{
    internal sealed class PaymentGatewayConfigurationHealthCheck(IOptions<PaymentGatewaySettings> paymentOptions) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(paymentOptions.Value.PrivateKey)
                ? HealthCheckResult.Unhealthy("PaymentSettings:PrivateKey is missing.")
                : HealthCheckResult.Healthy("Payment gateway authentication configuration is valid."));
        }
    }
}
