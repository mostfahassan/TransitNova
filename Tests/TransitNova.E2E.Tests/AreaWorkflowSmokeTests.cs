using Microsoft.Playwright;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class AreaWorkflowSmokeTests(TransitNovaBrowserFixture fixture)
{
    [Theory]
    [InlineData("admin.001@seed.transitnova.local", "AdminArea", "/AdminArea/Shipments/Index", "Shipments")]
    [InlineData("admin.001@seed.transitnova.local", "AdminArea", "/AdminArea/Subscriptions/BundleSubscribers", "Bundle subscribers")]
    [InlineData("admin.001@seed.transitnova.local", "AdminArea", "/AdminArea/Warehouses/Index", "Warehouses")]
    [InlineData("operation.manager.001@seed.transitnova.local", "OperationManagerArea", "/OperationManagerArea/Shipments/Index", "Shipments")]
    [InlineData("operation.manager.001@seed.transitnova.local", "OperationManagerArea", "/OperationManagerArea/Trips/Index", "Trips")]
    [InlineData("operation.manager.001@seed.transitnova.local", "OperationManagerArea", "/OperationManagerArea/Carriers/Index", "Carriers")]
    [InlineData("carrier.001@seed.transitnova.local", "CarrierArea", "/CarrierArea/Profile/AdditionalInfo", "profile")]
    [InlineData("warehouse.manager.001@seed.transitnova.local", "WarehouseManagerArea", "/WarehouseManagerArea/Shipments/Index", "Shipments")]
    [InlineData("warehouse.manager.001@seed.transitnova.local", "WarehouseManagerArea", "/WarehouseManagerArea/Trips/Index", "Trips")]
    public async Task RoleWorkflowPage_Should_LoadWithoutServerOrPermissionErrorAsync(
        string email,
        string area,
        string route,
        string expectedText)
    {
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();
        await fixture.LoginAsync(page, email, area);

        var response = await page.GotoAsync(route);

        Assert.True(response!.Ok, $"{route} returned {response.Status}.");
        await Assertions.Expect(page.Locator("body")).ToContainTextAsync(expectedText, new() { IgnoreCase = true });
        await Assertions.Expect(page.Locator("body")).Not.ToContainTextAsync("Have No Permission");
        await Assertions.Expect(page.Locator("body")).Not.ToContainTextAsync("An unhandled exception occurred");
    }

    [Fact]
    public async Task CreateShipment_Should_ShowValidationWithoutLosingFormAsync()
    {
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();
        await fixture.LoginAsync(page, "customer.001@seed.transitnova.local", "UserArea");
        await page.GotoAsync("/UserArea/Shipments/Create");

        await page.GetByRole(AriaRole.Button, new() { NameRegex = new("Create shipment", System.Text.RegularExpressions.RegexOptions.IgnoreCase) }).ClickAsync();

        await Assertions.Expect(page.Locator("[data-customer-create-form]")).ToBeVisibleAsync();
        await Assertions.Expect(page.Locator("[data-form-alert], .field-validation-error, .text-danger").First).ToBeVisibleAsync();
        Assert.Contains("/UserArea/Shipments/Create", page.Url, StringComparison.OrdinalIgnoreCase);
    }
}


