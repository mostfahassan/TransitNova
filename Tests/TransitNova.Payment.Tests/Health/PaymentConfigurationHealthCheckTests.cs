using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.InfraStructure.Health;

namespace TransitNova.Payment.Tests.Health;

public sealed class PaymentConfigurationHealthCheckTests
{
    [Fact]
    public async Task PaymentGatewayConfigurationHealthCheck_Should_ReportHealthy_When_PrivateKeyExistsAsync()
    {
        var healthCheck = new PaymentGatewayConfigurationHealthCheck(Options.Create(new PaymentGatewaySettings
        {
            PrivateKey = "payment-private-key"
        }));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task PaymentGatewayConfigurationHealthCheck_Should_ReportUnhealthy_When_PrivateKeyIsMissingAsync()
    {
        var healthCheck = new PaymentGatewayConfigurationHealthCheck(Options.Create(new PaymentGatewaySettings()));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Contain("PaymentSettings:PrivateKey");
    }

    [Fact]
    public async Task ObservabilityConfigurationHealthCheck_Should_RequireSeqAndOtlpInProductionAsync()
    {
        var healthCheck = new ObservabilityConfigurationHealthCheck(
            new ConfigurationBuilder().Build(),
            new TestHostEnvironment(Environments.Production));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Contain("Seq");
        result.Description.Should().Contain("OTEL_EXPORTER_OTLP_ENDPOINT");
    }

    private sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "TransitNova.Payment.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
