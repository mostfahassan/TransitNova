using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TransitNova.BusinessLayer.Options;
using TransitNova.InfraStructure.Health;

namespace TransitNova.InfraStructure.Tests.Health;

public sealed class ConfigurationHealthCheckTests
{
    [Fact]
    public async Task PaymentGatewayConfigurationHealthCheck_Should_ReportHealthy_When_SettingsAreValidAsync()
    {
        var healthCheck = new PaymentGatewayConfigurationHealthCheck(Options.Create(new PaymentSettings
        {
            PublicKey = "public-key",
            BaseUrl = "https://payments.test"
        }));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task PaymentGatewayConfigurationHealthCheck_Should_ReportUnhealthy_When_BaseUrlIsInvalidAsync()
    {
        var healthCheck = new PaymentGatewayConfigurationHealthCheck(Options.Create(new PaymentSettings
        {
            PublicKey = "public-key",
            BaseUrl = "not-a-url"
        }));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Contain("PaymentSettings:BaseUrl");
    }

    [Fact]
    public async Task ObservabilityConfigurationHealthCheck_Should_NotRequireEndpointsOutsideProductionAsync()
    {
        var healthCheck = new ObservabilityConfigurationHealthCheck(
            new ConfigurationBuilder().Build(),
            new TestHostEnvironment("Testing"));

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        result.Status.Should().Be(HealthStatus.Healthy);
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
        public string ApplicationName { get; set; } = "TransitNova.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
