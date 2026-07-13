using Microsoft.Playwright;
using System.Net;
using System.Text.Json;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class SignalRNotificationEndToEndTests(TransitNovaBrowserFixture fixture)
{
    [Fact]
    public async Task ApprovingShipment_ShouldPersistAndPushNotificationOnlyToOwningUserAsync()
    {
        using var ownerSession = await fixture.LoginApiAsync("customer.030@seed.transitnova.local");
        using var otherUserSession = await fixture.LoginApiAsync("customer.031@seed.transitnova.local");
        var cityId = await fixture.GetFirstCityIdAsync(ownerSession.Client);

        using var createRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/users/shipments",
            E2EData.CreateShipmentBody(cityId));
        using var createResponse = await ownerSession.Client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        using var createDocument = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var invoice = createDocument.RootElement.GetProperty("data");
        var shipmentId = invoice.GetProperty("shipmentId").GetGuid();
        var trackingNumber = invoice.GetProperty("shipmentTrackingNumber").GetString()!;

        await WaitForNotificationAsync(
            ownerSession.Client,
            notification => notification.GetProperty("message").GetString()!.Contains(trackingNumber, StringComparison.Ordinal));
        await MarkAllReadAsync(ownerSession.Client);
        await MarkAllReadAsync(otherUserSession.Client);

        await using var ownerContext = await fixture.NewContextAsync();
        var ownerPage = await ownerContext.NewPageAsync();
        await fixture.LoginAsync(ownerPage, "customer.030@seed.transitnova.local", "UserArea");
        await ownerPage.Locator("body[data-notifications-connection='connected']")
            .WaitForAsync(new() { Timeout = 30_000 });

        await using var otherContext = await fixture.NewContextAsync();
        var otherPage = await otherContext.NewPageAsync();
        await fixture.LoginAsync(otherPage, "customer.031@seed.transitnova.local", "UserArea");
        await otherPage.Locator("body[data-notifications-connection='connected']")
            .WaitForAsync(new() { Timeout = 30_000 });

        var ownerBadge = ownerPage.Locator("[data-notification-badge]");
        var otherBadge = otherPage.Locator("[data-notification-badge]");
        await Assertions.Expect(ownerBadge).ToHaveTextAsync("0");
        await Assertions.Expect(otherBadge).ToHaveTextAsync("0");

        using var operationManagerSession = await fixture.LoginApiAsync("operation.manager.001@seed.transitnova.local");
        using var approveRequest = E2EHttp.Idempotent(
            HttpMethod.Patch,
            $"/api/v1/operation-managers/shipments/{shipmentId}/approve");
        using var approveResponse = await operationManagerSession.Client.SendAsync(approveRequest);
        Assert.Equal(HttpStatusCode.OK, approveResponse.StatusCode);

        await Assertions.Expect(ownerBadge).ToHaveTextAsync("1", new() { Timeout = 30_000 });
        await Assertions.Expect(ownerBadge).ToBeVisibleAsync();

        var approvedNotification = await WaitForNotificationAsync(
            ownerSession.Client,
            notification => notification.GetProperty("title").GetString() == "Shipment Approved"
                && notification.GetProperty("message").GetString()!.Contains(trackingNumber, StringComparison.Ordinal));
        Assert.False(approvedNotification.GetProperty("isRead").GetBoolean());

        await Assertions.Expect(otherBadge).ToHaveTextAsync("0");
        await Assertions.Expect(otherBadge).ToBeHiddenAsync();
        Assert.False(await HasNotificationAsync(
            otherUserSession.Client,
            notification => notification.GetProperty("message").GetString()!.Contains(trackingNumber, StringComparison.Ordinal)));
    }

    private static async Task MarkAllReadAsync(HttpClient client)
    {
        using var response = await client.PatchAsync("/api/v1/notifications/read-all", null);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<JsonElement> WaitForNotificationAsync(
        HttpClient client,
        Func<JsonElement, bool> predicate)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        do
        {
            using var response = await client.GetAsync("/api/v1/notifications?pageNumber=1&pageSize=20");
            response.EnsureSuccessStatusCode();
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var notifications = document.RootElement.GetProperty("data").GetProperty("data");
            foreach (var notification in notifications.EnumerateArray())
            {
                if (predicate(notification))
                    return notification.Clone();
            }

            await Task.Delay(500);
        }
        while (DateTime.UtcNow < deadline);

        throw new TimeoutException("The expected persisted notification was not created within 30 seconds.");
    }

    private static async Task<bool> HasNotificationAsync(
        HttpClient client,
        Func<JsonElement, bool> predicate)
    {
        using var response = await client.GetAsync("/api/v1/notifications?pageNumber=1&pageSize=20");
        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement
            .GetProperty("data")
            .GetProperty("data")
            .EnumerateArray()
            .Any(predicate);
    }
}
