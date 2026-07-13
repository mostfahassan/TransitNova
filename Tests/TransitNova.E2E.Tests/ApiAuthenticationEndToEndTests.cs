using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TransitNova.E2E.Tests;

[Collection(E2ECollection.Name)]
public sealed class ApiAuthenticationEndToEndTests(TransitNovaBrowserFixture fixture)
{
    [Fact]
    public async Task Login_ShouldIssueTokensAndAuthorizeUserProfileAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.001@seed.transitnova.local");

        using var response = await session.Client.GetAsync("/api/v1/users/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = await ReadJsonAsync(response);
        Assert.True(document.RootElement.GetProperty("isSuccess").GetBoolean());
        Assert.Equal("customer.001@seed.transitnova.local", document.RootElement.GetProperty("data").GetProperty("email").GetString());
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorizedAsync()
    {
        using var client = fixture.CreateApiClient();

        using var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "customer.001@seed.transitnova.local",
            password = "Definitely-Wrong@123"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task RefreshToken_ShouldRotateTokenAndRejectOldTokenReuseAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.002@seed.transitnova.local");
        using var client = fixture.CreateApiClient();

        using var firstRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/refresh-tokens",
            new { token = session.RefreshToken });
        using var firstResponse = await client.SendAsync(firstRequest);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        using var firstDocument = await ReadJsonAsync(firstResponse);
        var rotatedRefreshToken = firstDocument.RootElement.GetProperty("data").GetProperty("refreshToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(rotatedRefreshToken));
        Assert.NotEqual(session.RefreshToken, rotatedRefreshToken);

        using var reuseRequest = E2EHttp.Idempotent(
            HttpMethod.Post,
            "/api/v1/refresh-tokens",
            new { token = session.RefreshToken });
        using var reuseResponse = await client.SendAsync(reuseRequest);

        Assert.Equal(HttpStatusCode.Forbidden, reuseResponse.StatusCode);
    }

    [Fact]
    public async Task AnonymousAndWrongRoleRequests_ShouldBeRejectedAsync()
    {
        using var anonymousClient = fixture.CreateApiClient();
        using var anonymousResponse = await anonymousClient.GetAsync("/api/v1/notifications");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        using var userSession = await fixture.LoginApiAsync("customer.003@seed.transitnova.local");
        using var forbiddenResponse = await userSession.Client.GetAsync("/api/v1/admin/warehouses");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
    }

    [Fact]
    public async Task InvalidRegistration_ShouldReturnValidationProblemAsync()
    {
        using var client = fixture.CreateApiClient();
        using var request = E2EHttp.Idempotent(HttpMethod.Post, "/api/v1/auth/register", new
        {
            userName = "x",
            email = "not-an-email",
            password = "weak",
            confirmPassword = "different",
            phoneNumber = "invalid",
            firstName = "1",
            lastName = "2",
            address = new { mainAddress = "", street = "" },
            userType = "User",
            cityId = 0
        });

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        using var document = await ReadJsonAsync(response);
        Assert.True(document.RootElement.TryGetProperty("errors", out _));
    }

    [Fact]
    public async Task Registration_ShouldPersistUserAndRejectDuplicateEmailAsync()
    {
        using var client = fixture.CreateApiClient();
        var cityId = await fixture.GetFirstCityIdAsync(client);
        var runSeed = Environment.GetEnvironmentVariable("TRANSITNOVA_E2E_RUN_ID") ?? "local";
        var suffix = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(runSeed))).ToLowerInvariant()[..12];
        var email = $"e2e.{suffix}@transitnova.test";
        var password = "SafeE2e@4826";
        var registration = new
        {
            userName = $"e2e_{suffix[..12]}",
            email,
            password,
            confirmPassword = password,
            phoneNumber = "+201001234567",
            firstName = "Endtoend",
            lastName = "Tester",
            address = new
            {
                mainAddress = "TransitNova E2E address",
                secondaryAddress = "CI suite",
                street = "Automation Street"
            },
            userType = "User",
            cityId
        };

        using var registerRequest = E2EHttp.Idempotent(HttpMethod.Post, "/api/v1/auth/register", registration);
        using var registerResponse = await client.SendAsync(registerRequest);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        using var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var duplicateRequest = E2EHttp.Idempotent(HttpMethod.Post, "/api/v1/auth/register", registration);
        using var duplicateResponse = await client.SendAsync(duplicateRequest);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task SignOut_ShouldReturnSuccessfulEnvelopeAsync()
    {
        using var session = await fixture.LoginApiAsync("customer.004@seed.transitnova.local");
        using var request = E2EHttp.Idempotent(HttpMethod.Post, "/api/v1/auth/signout");

        using var response = await session.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = await ReadJsonAsync(response);
        Assert.True(document.RootElement.GetProperty("isSuccess").GetBoolean());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
        => JsonDocument.Parse(await response.Content.ReadAsStringAsync());
}





