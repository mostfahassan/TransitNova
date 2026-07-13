using System.Net;
using System.Text.Json;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class ApiRoleReadWorkflowTests(TransitNovaBrowserFixture fixture)
{
    [Theory]
    [InlineData("admin.001@seed.transitnova.local", "/api/v1/admin/shipments?pageNumber=1&pageSize=5")]
    [InlineData("admin.001@seed.transitnova.local", "/api/v1/admin/warehouses")]
    [InlineData("admin.001@seed.transitnova.local", "/api/v1/admin/subscriptions/subscribers")]
    [InlineData("operation.manager.001@seed.transitnova.local", "/api/v1/operation-managers/shipments?pageNumber=1&pageSize=5")]
    [InlineData("operation.manager.001@seed.transitnova.local", "/api/v1/operation-managers/shipments/review-queue?pageNumber=1&pageSize=5")]
    [InlineData("operation.manager.001@seed.transitnova.local", "/api/v1/operation-managers/trips?pageNumber=1&pageSize=5")]
    [InlineData("operation.manager.001@seed.transitnova.local", "/api/v1/operation-managers/carriers?pageNumber=1&pageSize=5")]
    [InlineData("customer.001@seed.transitnova.local", "/api/v1/notifications?pageNumber=1&pageSize=5")]
    [InlineData("carrier.001@seed.transitnova.local", "/api/v1/notifications/unread-count")]
    [InlineData("warehouse.manager.001@seed.transitnova.local", "/api/v1/notifications/unread-count")]
    public async Task RoleScopedReadEndpoint_ShouldReturnValidSuccessEnvelopeAsync(string email, string route)
    {
        using var session = await fixture.LoginApiAsync(email);

        using var response = await session.Client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.True(root.GetProperty("isSuccess").GetBoolean());
        Assert.Equal(200, root.GetProperty("statusCode").GetInt32());
        Assert.True(root.TryGetProperty("data", out var data));
        Assert.NotEqual(JsonValueKind.Undefined, data.ValueKind);
    }

    [Fact]
    public async Task AdminShipmentListAndDetails_ShouldPreserveResponseContractAsync()
    {
        using var session = await fixture.LoginApiAsync("admin.001@seed.transitnova.local");
        using var listResponse = await session.Client.GetAsync("/api/v1/admin/shipments?pageNumber=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        using var listDocument = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        var items = listDocument.RootElement.GetProperty("data").GetProperty("data");
        Assert.Equal(JsonValueKind.Array, items.ValueKind);
        Assert.NotEmpty(items.EnumerateArray());
        var shipmentId = items[0].GetProperty("id").GetGuid();

        using var detailsResponse = await session.Client.GetAsync($"/api/v1/admin/shipments/{shipmentId}");

        Assert.Equal(HttpStatusCode.OK, detailsResponse.StatusCode);
        using var detailsDocument = JsonDocument.Parse(await detailsResponse.Content.ReadAsStringAsync());
        var details = detailsDocument.RootElement.GetProperty("data");
        Assert.Equal(shipmentId, details.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task ShipmentFilteringAndPagination_ShouldReturnBoundedMatchingPageAsync()
    {
        using var session = await fixture.LoginApiAsync("admin.001@seed.transitnova.local");

        using var response = await session.Client.GetAsync(
            "/api/v1/admin/shipments?pageNumber=1&pageSize=3&status=Pending&searchTerm=receiver");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var page = document.RootElement.GetProperty("data");
        var items = page.GetProperty("data");
        Assert.InRange(items.GetArrayLength(), 0, 3);
        Assert.Equal(1, page.GetProperty("pageNumber").GetInt32());
        Assert.Equal(3, page.GetProperty("pageSize").GetInt32());
    }

    [Fact]
    public async Task CarrierSorting_ShouldReturnRatingsInDescendingOrderAsync()
    {
        using var session = await fixture.LoginApiAsync("operation.manager.001@seed.transitnova.local");

        using var response = await session.Client.GetAsync(
            "/api/v1/operation-managers/carriers?sortBy=Rating&sortDescending=true&pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var ratings = document.RootElement
            .GetProperty("data")
            .GetProperty("data")
            .EnumerateArray()
            .Select(item => item.GetProperty("rating").GetDecimal())
            .ToArray();
        Assert.NotEmpty(ratings);
        Assert.Equal(ratings.OrderByDescending(value => value), ratings);
    }

    [Fact]
    public async Task OperationManagerTripListAndDetails_ShouldUseSameScopedTripAsync()
    {
        using var session = await fixture.LoginApiAsync("operation.manager.001@seed.transitnova.local");
        using var listResponse = await session.Client.GetAsync(
            "/api/v1/operation-managers/trips?pageNumber=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        using var listDocument = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        var trips = listDocument.RootElement.GetProperty("data").GetProperty("data");
        Assert.NotEmpty(trips.EnumerateArray());
        var tripId = trips[0].GetProperty("id").GetGuid();

        using var detailsResponse = await session.Client.GetAsync(
            $"/api/v1/operation-managers/trips/{tripId}");

        Assert.Equal(HttpStatusCode.OK, detailsResponse.StatusCode);
        using var detailsDocument = JsonDocument.Parse(await detailsResponse.Content.ReadAsStringAsync());
        Assert.Equal(tripId, detailsDocument.RootElement.GetProperty("data").GetProperty("id").GetGuid());
    }
    [Fact]
    public async Task NotificationsReadAll_ShouldResetUnreadCountForCurrentUserAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.005@seed.transitnova.local");
        using var markReadResponse = await session.Client.PatchAsync("/api/v1/notifications/read-all", null);
        Assert.Equal(HttpStatusCode.OK, markReadResponse.StatusCode);

        using var countResponse = await session.Client.GetAsync("/api/v1/notifications/unread-count");
        Assert.Equal(HttpStatusCode.OK, countResponse.StatusCode);
        using var document = JsonDocument.Parse(await countResponse.Content.ReadAsStringAsync());
        Assert.Equal(0, document.RootElement.GetProperty("data").GetProperty("count").GetInt32());
    }
}


