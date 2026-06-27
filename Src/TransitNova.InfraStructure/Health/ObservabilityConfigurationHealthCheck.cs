using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace TransitNova.InfraStructure.Health
{
    internal sealed class ObservabilityConfigurationHealthCheck(
        IConfiguration configuration,
        IHostEnvironment environment) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (!environment.IsProduction())
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Observability endpoint validation is required only in Production."));
            }

            var failures = new List<string>();

            if (!IsAbsoluteHttpUrl(GetSeqServerUrl(configuration)))
                failures.Add("Serilog Seq serverUrl is missing or invalid.");

            if (!IsAbsoluteHttpUrl(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
                failures.Add("OTEL_EXPORTER_OTLP_ENDPOINT is missing or invalid.");

            return Task.FromResult(failures.Count == 0
                ? HealthCheckResult.Healthy("Production observability configuration is valid.")
                : HealthCheckResult.Unhealthy(string.Join(' ', failures)));
        }

        private static string? GetSeqServerUrl(IConfiguration configuration)
        {
            foreach (var sink in configuration.GetSection("Serilog:WriteTo").GetChildren())
            {
                if (string.Equals(sink["Name"], "Seq", StringComparison.OrdinalIgnoreCase))
                    return sink.GetSection("Args")["serverUrl"];
            }

            return configuration["Seq:ServerUrl"];
        }

        private static bool IsAbsoluteHttpUrl(string? value) =>
            Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            uri.Scheme is "http" or "https";
    }
}
