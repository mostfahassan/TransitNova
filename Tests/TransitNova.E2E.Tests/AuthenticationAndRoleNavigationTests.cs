using Microsoft.Playwright;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class AuthenticationAndRoleNavigationTests(TransitNovaBrowserFixture fixture)
{
    [Fact]
    public async Task LandingAndLoginPages_Should_RenderWithoutClientErrorsAsync()
    {
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();
        var errors = new List<string>();
        page.Console += (_, message) =>
        {
            if (message.Type == "error") errors.Add(message.Text);
        };

        var landing = await page.GotoAsync("/");
        await Assertions.Expect(page.Locator("body")).ToContainTextAsync("TransitNova");
        var login = await page.GotoAsync("/AccountArea/Account/Login");
        await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Welcome back." })).ToBeVisibleAsync();

        Assert.True(landing!.Ok);
        Assert.True(login!.Ok);
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("/UserArea/Dashboard/Index")]
    [InlineData("/AdminArea/Dashboard/Index")]
    [InlineData("/CarrierArea/Dashboard/Index")]
    [InlineData("/OperationManagerArea/Dashboard/Index")]
    [InlineData("/WarehouseManagerArea/Dashboard/Index")]
    public async Task ProtectedDashboard_Should_RedirectAnonymousVisitorToLoginAsync(string route)
    {
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync(route);

        await page.WaitForURLAsync(url => url.Contains("/AccountArea/Account/Login", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("customer.001@seed.transitnova.local", "UserArea")]
    [InlineData("admin.001@seed.transitnova.local", "AdminArea")]
    [InlineData("carrier.001@seed.transitnova.local", "CarrierArea")]
    [InlineData("operation.manager.001@seed.transitnova.local", "OperationManagerArea")]
    [InlineData("warehouse.manager.001@seed.transitnova.local", "WarehouseManagerArea")]
    public async Task Login_Should_RouteEachRoleToItsDashboardAsync(string email, string area)
    {
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();

        await fixture.LoginAsync(page, email, area);

        Assert.Contains($"/{area}/Dashboard", page.Url, StringComparison.OrdinalIgnoreCase);
        await Assertions.Expect(page.Locator("body")).Not.ToContainTextAsync("An unhandled exception occurred");
    }

    [Theory]
    [InlineData("customer.001@seed.transitnova.local", "UserArea")]
    [InlineData("admin.001@seed.transitnova.local", "AdminArea")]
    [InlineData("carrier.001@seed.transitnova.local", "CarrierArea")]
    [InlineData("operation.manager.001@seed.transitnova.local", "OperationManagerArea")]
    [InlineData("warehouse.manager.001@seed.transitnova.local", "WarehouseManagerArea")]
    public async Task NotificationCenter_Should_OpenForEveryRoleAsync(string email, string area)
    {
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();
        await fixture.LoginAsync(page, email, area);

        var response = await page.GotoAsync($"/{area}/Notifications/Index");

        Assert.True(response!.Ok, $"Notifications returned {response.Status} for {area}.");
        await Assertions.Expect(page.Locator("body")).ToContainTextAsync("Notifications");
        await Assertions.Expect(page.Locator("body")).Not.ToContainTextAsync("An unhandled exception occurred");
    }
}
