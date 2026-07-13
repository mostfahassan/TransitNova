using System.Net;
using System.Text.Json;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class ShipmentLifecycleEndToEndTests(TransitNovaBrowserFixture fixture)
{
    [Fact]
    public async Task CreateUpdateTrackAndCancel_ShouldPersistEachShipmentTransitionAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.010@seed.transitnova.local");
        var cityId = await fixture.GetFirstCityIdAsync(session.Client);
        var createBody = E2EData.CreateShipmentBody(cityId);
        var createRequestId = Guid.NewGuid();

        using var createRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/users/shipments",
            createBody,
            createRequestId);
        using var createResponse = await session.Client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        using var createDocument = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var invoice = createDocument.RootElement.GetProperty("data");
        var shipmentId = invoice.GetProperty("shipmentId").GetGuid();
        var trackingNumber = invoice.GetProperty("shipmentTrackingNumber").GetString();
        Assert.NotEqual(Guid.Empty, shipmentId);
        Assert.False(string.IsNullOrWhiteSpace(trackingNumber));
        Assert.True(invoice.GetProperty("totalAmount").GetDecimal() > 0);

        using var retryRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/users/shipments",
            createBody,
            createRequestId);
        using var retryResponse = await session.Client.SendAsync(retryRequest);
        Assert.Equal(HttpStatusCode.Created, retryResponse.StatusCode);
        using var retryDocument = JsonDocument.Parse(await retryResponse.Content.ReadAsStringAsync());
        Assert.Equal(shipmentId, retryDocument.RootElement.GetProperty("data").GetProperty("shipmentId").GetGuid());

        using var detailsBeforeUpdate = await session.Client.GetAsync($"/api/v1/users/shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, detailsBeforeUpdate.StatusCode);

        var updateBody = new
        {
            receiverId = (Guid?)null,
            deliveryAddress = (object?)null,
            pickupAddress = (object?)null,
            packageSpecification = new { weight = 4.5m, width = 25m, height = 18m, length = 35m },
            shipmentType = "Express",
            transportationMode = "Air"
        };
        using var updateRequest = E2EHttp.Idempotent(
            HttpMethod.Put,
            $"/api/v1/users/shipments/{shipmentId}",
            updateBody);
        using var updateResponse = await session.Client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using var detailsAfterUpdate = await session.Client.GetAsync($"/api/v1/users/shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, detailsAfterUpdate.StatusCode);
        using var updatedDocument = JsonDocument.Parse(await detailsAfterUpdate.Content.ReadAsStringAsync());
        var updatedShipment = updatedDocument.RootElement.GetProperty("data");
        Assert.Equal("Express", updatedShipment.GetProperty("shipmentType").GetString());
        Assert.Equal("Air", updatedShipment.GetProperty("transportationMode").GetString());
        Assert.Equal(4.5m, updatedShipment.GetProperty("packageSpecification").GetProperty("weight").GetDecimal());

        using var trackResponse = await session.Client.GetAsync($"/api/v1/users/shipments/{trackingNumber}");
        Assert.Equal(HttpStatusCode.OK, trackResponse.StatusCode);

        var cancelRequestId = Guid.NewGuid();
        using var cancelRequest = E2EHttp.Idempotent(
            HttpMethod.Patch,
            $"/api/v1/users/shipments/{shipmentId}/cancel",
            requestId: cancelRequestId);
        using var cancelResponse = await session.Client.SendAsync(cancelRequest);
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        using var cancelRetryRequest = E2EHttp.Idempotent(
            HttpMethod.Patch,
            $"/api/v1/users/shipments/{shipmentId}/cancel",
            requestId: cancelRequestId);
        using var cancelRetryResponse = await session.Client.SendAsync(cancelRetryRequest);
        Assert.Equal(HttpStatusCode.OK, cancelRetryResponse.StatusCode);

        using var cancelledDetailsResponse = await session.Client.GetAsync($"/api/v1/users/shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, cancelledDetailsResponse.StatusCode);
        using var cancelledDocument = JsonDocument.Parse(await cancelledDetailsResponse.Content.ReadAsStringAsync());
        Assert.Equal("Cancelled", cancelledDocument.RootElement.GetProperty("data").GetProperty("currentStatus").GetString());
    }

    [Fact]
    public async Task ReusingIdempotencyKeyWithDifferentShipmentPayload_ShouldReturnConflictAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.011@seed.transitnova.local");
        var cityId = await fixture.GetFirstCityIdAsync(session.Client);
        var requestId = Guid.NewGuid();

        using var firstRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/users/shipments",
            E2EData.CreateShipmentBody(cityId),
            requestId);
        using var firstResponse = await session.Client.SendAsync(firstRequest);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        using var firstDocument = JsonDocument.Parse(await firstResponse.Content.ReadAsStringAsync());
        var shipmentId = firstDocument.RootElement.GetProperty("data").GetProperty("shipmentId").GetGuid();

        using var conflictingRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/users/shipments",
            E2EData.CreateShipmentBody(cityId, weight: 9.25m),
            requestId);
        using var conflictingResponse = await session.Client.SendAsync(conflictingRequest);

        Assert.Equal(HttpStatusCode.Conflict, conflictingResponse.StatusCode);

        using var deleteRequest = E2EHttp.Idempotent(
            HttpMethod.Delete,
            $"/api/v1/users/shipments/{shipmentId}");
        using var deleteResponse = await session.Client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        using var deletedDetailsResponse = await session.Client.GetAsync($"/api/v1/users/shipments/{shipmentId}");
        Assert.Equal(HttpStatusCode.OK, deletedDetailsResponse.StatusCode);
        using var deletedDocument = JsonDocument.Parse(await deletedDetailsResponse.Content.ReadAsStringAsync());
        Assert.Equal("Deleted", deletedDocument.RootElement.GetProperty("data").GetProperty("currentStatus").GetString());
    }

}


