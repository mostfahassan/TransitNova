using Microsoft.Playwright;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TransitNova.E2E.Tests;

public sealed class TransitNovaBrowserFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public string BaseUrl { get; } =
        (Environment.GetEnvironmentVariable("TRANSITNOVA_E2E_BASE_URL") ?? "http://localhost:5169").TrimEnd('/');

    public string ApiBaseUrl { get; } =
        (Environment.GetEnvironmentVariable("TRANSITNOVA_E2E_API_BASE_URL") ?? "http://localhost:5200").TrimEnd('/');

    public async Task InitializeAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        using var response = await client.GetAsync(BaseUrl);
        response.EnsureSuccessStatusCode();

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
            await _browser.DisposeAsync();
        _playwright?.Dispose();
    }

    public async Task<IBrowserContext> NewContextAsync()
    {
        if (_browser is null)
            throw new InvalidOperationException("The E2E browser has not been initialized.");

        return await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            ViewportSize = new ViewportSize { Width = 1440, Height = 900 },
            Locale = "en-US"
        });
    }

    public async Task LoginAsync(IPage page, string email, string expectedArea)
    {
        await page.GotoAsync("/AccountArea/Account/Login");
        await page.Locator("input[name='Email']").FillAsync(email);
        await page.Locator("input[name='Password']").FillAsync("TransitNova@12345");
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();
        await page.WaitForURLAsync(url => url.Contains($"/{expectedArea}/Dashboard", StringComparison.OrdinalIgnoreCase));
    }

    public HttpClient CreateApiClient()
        => new() { BaseAddress = new Uri(ApiBaseUrl), Timeout = TimeSpan.FromSeconds(30) };

    public async Task<int> GetFirstCityIdAsync(HttpClient client)
    {
        using var countriesResponse = await client.GetAsync("/api/v1/countries");
        countriesResponse.EnsureSuccessStatusCode();
        using var countries = JsonDocument.Parse(await countriesResponse.Content.ReadAsStringAsync());
        var countryId = countries.RootElement.GetProperty("data")[0].GetProperty("id").GetInt32();

        using var governmentsResponse = await client.GetAsync($"/api/v1/countries/{countryId}/governments");
        governmentsResponse.EnsureSuccessStatusCode();
        using var governments = JsonDocument.Parse(await governmentsResponse.Content.ReadAsStringAsync());
        var governmentId = governments.RootElement.GetProperty("data")[0].GetProperty("id").GetInt32();

        using var citiesResponse = await client.GetAsync($"/api/v1/governments/{governmentId}/cities");
        citiesResponse.EnsureSuccessStatusCode();
        using var cities = JsonDocument.Parse(await citiesResponse.Content.ReadAsStringAsync());
        return cities.RootElement.GetProperty("data")[0].GetProperty("id").GetInt32();
    }
    public async Task<ApiSession> LoginApiAsync(string email)
    {
        var client = CreateApiClient();
        using var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email,
            password = "TransitNova@12345"
        });

        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = document.RootElement.GetProperty("data");
        var token = data.GetProperty("token").GetString();
        var refreshToken = data.GetProperty("refreshToken").GetString();
        var userId = data.GetProperty("id").GetGuid();

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
            throw new InvalidOperationException($"Login did not return both tokens for '{email}'.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return new ApiSession(client, userId, token, refreshToken);
    }
}

public sealed record ApiSession(
    HttpClient Client,
    Guid UserId,
    string AccessToken,
    string RefreshToken) : IDisposable
{
    public void Dispose() => Client.Dispose();
}


