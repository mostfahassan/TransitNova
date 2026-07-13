using System.Net;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class ErrorAndReportEndToEndTests(TransitNovaBrowserFixture fixture)
{
    [Fact]
    public async Task DownloadingUnknownReport_ShouldReturnNotFoundAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.020@seed.transitnova.local");

        using var response = await session.Client.GetAsync($"/api/v1/reports/{Guid.NewGuid()}/download");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task InvalidShipmentReportRequest_ShouldReturnValidationErrorsAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.021@seed.transitnova.local");
        using var request = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/reports/shipments",
            new { shipmentId = Guid.Empty });

        using var response = await session.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DashboardReport_ShouldEnforceRoleAndHonorIdempotentRetryAsync()
    {
        using var userSession = await fixture.LoginApiAsync("customer.022@seed.transitnova.local");
        using var forbiddenRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/reports/dashboards",
            new { });
        using var forbiddenResponse = await userSession.Client.SendAsync(forbiddenRequest);
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);

        using var adminSession = await fixture.LoginApiAsync("admin.001@seed.transitnova.local");
        var requestId = Guid.NewGuid();
        using var firstRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/reports/dashboards",
            new { },
            requestId);
        using var firstResponse = await adminSession.Client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        using var retryRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/reports/dashboards",
            new { },
            requestId);
        using var retryResponse = await adminSession.Client.SendAsync(retryRequest);
        Assert.Equal(HttpStatusCode.Created, retryResponse.StatusCode);
        Assert.Equal(
            await firstResponse.Content.ReadAsStringAsync(),
            await retryResponse.Content.ReadAsStringAsync());
    }
}
