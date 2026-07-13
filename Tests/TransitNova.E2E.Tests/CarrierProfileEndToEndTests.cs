using System.Net;
using System.Text.Json;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class CarrierProfileEndToEndTests(TransitNovaBrowserFixture fixture)
{
    [Fact]
    public async Task CarrierProfileUpdate_ShouldRespectOwnershipAndInvalidateCachedProfileAsync()
    {
        var carrierId = await ResolveCarrierIdAsync("CR-SEED-001");
        using var carrierSession = await fixture.LoginApiAsync("carrier.001@seed.transitnova.local");

        using var initialProfileResponse = await carrierSession.Client.GetAsync($"/api/v1/carriers/{carrierId}/profile");
        Assert.Equal(HttpStatusCode.OK, initialProfileResponse.StatusCode);

        using var dashboardResponse = await carrierSession.Client.GetAsync($"/api/v1/carriers/{carrierId}/dashboard");
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);

        const string updatedPhone = "+201099999991";
        using var updateRequest = E2EHttp.Idempotent(
            HttpMethod.Put,
            "/api/v1/carriers/profile",
            new
            {
                id = carrierId,
                firstName = (string?)null,
                lastName = (string?)null,
                phoneNumber = updatedPhone,
                email = (string?)null,
                cityId = (int?)null,
                address = (object?)null
            });
        using var updateResponse = await carrierSession.Client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using var refreshedProfileResponse = await carrierSession.Client.GetAsync($"/api/v1/carriers/{carrierId}/profile");
        Assert.Equal(HttpStatusCode.OK, refreshedProfileResponse.StatusCode);
        using var refreshedDocument = JsonDocument.Parse(await refreshedProfileResponse.Content.ReadAsStringAsync());
        Assert.Equal(updatedPhone, refreshedDocument.RootElement.GetProperty("data").GetProperty("phoneNumber").GetString());

        using var foreignProfileResponse = await carrierSession.Client.GetAsync($"/api/v1/carriers/{Guid.NewGuid()}/profile");
        Assert.Equal(HttpStatusCode.Forbidden, foreignProfileResponse.StatusCode);
    }

    private async Task<Guid> ResolveCarrierIdAsync(string code)
    {
        using var operationManagerSession = await fixture.LoginApiAsync("operation.manager.001@seed.transitnova.local");
        using var response = await operationManagerSession.Client.GetAsync(
            $"/api/v1/operation-managers/carriers?searchTerm={Uri.EscapeDataString(code)}&pageNumber=1&pageSize=5");
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var carriers = document.RootElement.GetProperty("data").GetProperty("data");
        Assert.Single(carriers.EnumerateArray());
        return carriers[0].GetProperty("id").GetGuid();
    }
}
