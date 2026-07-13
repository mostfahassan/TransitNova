using System.Net;
using TransitNova.Api.IntegrationTests.Infrastructure;

namespace TransitNova.Api.IntegrationTests;

public sealed class SignalRCorsIntegrationTests(TransitNovaWebApplicationFactory factory)
    : IClassFixture<TransitNovaWebApplicationFactory>
{
    [Fact]
    public async Task NotificationHubPreflight_ShouldAllowConfiguredUiOriginAndSignalRHeaderAsync()
    {
        using var client = factory.CreateAnonymousClient();
        using var request = new HttpRequestMessage(
            HttpMethod.Options,
            "/hubs/notifications/negotiate?negotiateVersion=1");
        request.Headers.Add("Origin", "https://localhost");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "authorization,x-signalr-user-agent");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(
            response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins),
            $"CORS origin header missing. Response headers: {response.Headers}");
        Assert.Equal("https://localhost", origins.Single());
        var allowedHeaders = string.Join(',', response.Headers.GetValues("Access-Control-Allow-Headers"));
        Assert.Contains("authorization", allowedHeaders, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("x-signalr-user-agent", allowedHeaders, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NotificationHubNegotiate_ShouldRejectAnonymousCallerAsync()
    {
        using var client = factory.CreateAnonymousClient();
        using var response = await client.PostAsync(
            "/hubs/notifications/negotiate?negotiateVersion=1",
            content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
