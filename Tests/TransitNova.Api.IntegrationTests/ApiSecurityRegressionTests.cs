using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TransitNova.Api.IntegrationTests.Infrastructure;
using TransitNova.BusinessLayer.DTOs.RefreshToken;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.InfraStructure.Context;
using RefreshTokenEntity = TransitNova.InfraStructure.RefreshToken;

namespace TransitNova.Api.IntegrationTests;

public sealed class ApiSecurityRegressionTests : IClassFixture<TransitNovaWebApplicationFactory>, IAsyncLifetime
{
    private readonly TransitNovaWebApplicationFactory _factory;

    public ApiSecurityRegressionTests(TransitNovaWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => _factory.InitializeDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RefreshTokenEndpoint_Should_AllowAnonymousRefresh_When_TokenIsValidAsync()
    {
        using var client = _factory.CreateAnonymousClient();
        var loginResponse = await LoginKnownUserAsync(client);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/refresh-tokens")
        {
            Content = JsonContent.Create(new RefreshToken(loginResponse.RefreshToken))
        };
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var refreshed = await ReadSuccessDataAsync<AuthResponseDto>(response);
        refreshed.IsAuthenticated.Should().BeTrue();
        refreshed.Id.Should().Be(loginResponse.Id);
        refreshed.Token.Should().NotBeNullOrWhiteSpace();
        refreshed.RefreshToken.Should().NotBeNullOrWhiteSpace();
        refreshed.RefreshToken.Should().NotBe(loginResponse.RefreshToken);
    }

    [Fact]
    public async Task RefreshTokenEndpoint_Should_ReturnForbidden_When_RevokedToken_IsReusedAsync()
    {
        using var client = _factory.CreateAnonymousClient();
        var loginResponse = await LoginKnownUserAsync(client);

        using var firstRefreshRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/refresh-tokens")
        {
            Content = JsonContent.Create(new RefreshToken(loginResponse.RefreshToken))
        };
        firstRefreshRequest.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var firstRefreshResponse = await client.SendAsync(firstRefreshRequest);
        firstRefreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var replayRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/refresh-tokens")
        {
            Content = JsonContent.Create(new RefreshToken(loginResponse.RefreshToken))
        };
        replayRequest.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var replayResponse = await client.SendAsync(replayRequest);

        replayResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RevokeRefreshTokenEndpoint_Should_ReturnUnauthorized_ForAnonymousAsync()
    {
        using var client = _factory.CreateAnonymousClient();
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/refresh-tokens/{TestAuthenticationHandler.UserId}");
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeRefreshTokenEndpoint_Should_ReturnForbidden_ForNonOwnerAsync()
    {
        using var client = _factory.CreateAuthenticatedClient(roles: ["User"], permissions: [], bypassAuthorization: false);
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/refresh-tokens/{Guid.NewGuid()}");
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RevokeRefreshTokenEndpoint_Should_RevokeOwnedTokens_ForOwnerAsync()
    {
        await SeedRefreshTokenAsync(TestAuthenticationHandler.UserId, $"owned-token-{Guid.NewGuid():N}");

        using var client = _factory.CreateAuthenticatedClient(roles: ["User"], permissions: [], bypassAuthorization: false);
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/refresh-tokens/{TestAuthenticationHandler.UserId}");
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasTokens = context.RefreshTokens.Any(token => token.UserId == TestAuthenticationHandler.UserId);
        hasTokens.Should().BeFalse();
    }

    [Fact]
    public async Task AdminEndpoint_Should_EnforceRoleBasedAccessAsync()
    {
        using var userClient = _factory.CreateAuthenticatedClient(roles: ["User"], permissions: [], bypassAuthorization: false);
        using var userResponse = await userClient.GetAsync("/api/v1/admin/bundles");
        userResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        using var adminClient = _factory.CreateAuthenticatedClient(roles: ["Admin"], permissions: [], bypassAuthorization: false);
        using var adminResponse = await adminClient.GetAsync("/api/v1/admin/bundles");
        adminResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<AuthResponseDto> LoginKnownUserAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginDto(TransitNovaWebApplicationFactory.KnownUserPassword, TransitNovaWebApplicationFactory.KnownUserEmail));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await ReadSuccessDataAsync<AuthResponseDto>(response);
        authResponse.IsAuthenticated.Should().BeTrue();
        authResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        return authResponse;
    }

    private async Task SeedRefreshTokenAsync(Guid userId, string token)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.RefreshTokens.Add(new RefreshTokenEntity
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        await context.SaveChangesAsync();
    }

    private static async Task<T> ReadSuccessDataAsync<T>(HttpResponseMessage response)
    {
        var envelope = await response.Content.ReadFromJsonAsync<SuccessEnvelope<T>>();
        envelope.Should().NotBeNull();
        envelope!.IsSuccess.Should().BeTrue();
        envelope.Data.Should().NotBeNull();
        return envelope.Data!;
    }

    private sealed class SuccessEnvelope<T>
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }
        public int StatusCode { get; init; }
        public T? Data { get; init; }
    }
}
