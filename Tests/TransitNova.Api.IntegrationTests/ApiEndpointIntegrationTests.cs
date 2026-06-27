using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Net.Http.Json;
using TransitNova.Api.IntegrationTests.Infrastructure;

namespace TransitNova.Api.IntegrationTests;

public sealed class ApiEndpointIntegrationTests : IClassFixture<TransitNovaWebApplicationFactory>, IAsyncLifetime
{
    private static readonly HashSet<HttpStatusCode> InvalidAuthenticatedStatuses =
    [
        HttpStatusCode.Unauthorized,
        HttpStatusCode.Forbidden,
        HttpStatusCode.MethodNotAllowed,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    ];

    private readonly TransitNovaWebApplicationFactory _factory;

    public ApiEndpointIntegrationTests(TransitNovaWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => _factory.InitializeDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ControllerEndpointCatalog_Should_DiscoverEveryCurrentApiAction()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services);
        var signatures = ControllerEndpointCatalogSnapshot.CreateSurfaceSignatures(endpoints);
        var checksum = ControllerEndpointCatalogSnapshot.ComputeChecksum(signatures);

        signatures.Should().HaveCount(ControllerEndpointCatalogSnapshot.ExpectedEndpointCount);
        checksum.Should().Be(
            ControllerEndpointCatalogSnapshot.ExpectedSurfaceChecksum,
            $"public API surface changed; update {nameof(ControllerEndpointCatalogSnapshot)} only for intentional endpoint additions/removals. Current checksum: {checksum}");
        signatures.Should().OnlyHaveUniqueItems();
        endpoints
            .Where(endpoint => endpoint.RouteTemplate.Contains(":string", StringComparison.OrdinalIgnoreCase))
            .Select(endpoint => $"{endpoint.RouteTemplate} ({endpoint.ActionDescriptor.ControllerTypeInfo.FullName})")
            .Should().BeEmpty();
    }

    [Fact]
    public async Task EveryControllerEndpoint_Should_ExecuteThroughTheHttpPipelineAsync()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services);
        var failures = new List<string>();

        foreach (var endpoint in endpoints)
        {
            using var client = _factory.CreateAuthenticatedClient(
                $"route-{Guid.NewGuid():N}");
            using var request = CreateRequest(endpoint, Guid.NewGuid().ToString());
            using var response = await client.SendAsync(request);

            if (InvalidAuthenticatedStatuses.Contains(response.StatusCode) ||
                (int)response.StatusCode >= 500)
            {
                var body = await response.Content.ReadAsStringAsync();
                failures.Add(
                    $"{endpoint.HttpMethod} {endpoint.RequestPath} returned {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
            }
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task ProtectedControllerEndpoints_Should_ReturnUnauthorized_When_RequestIsAnonymousAsync()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services)
            .Where(endpoint => endpoint.RequiresAuthorization)
            .ToArray();
        var failures = new List<string>();

        using var client = _factory.CreateAnonymousClient();
        foreach (var endpoint in endpoints)
        {
            using var request = CreateRequest(endpoint, Guid.NewGuid().ToString());
            using var response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                var body = await response.Content.ReadAsStringAsync();
                failures.Add(
                    $"{endpoint.HttpMethod} {endpoint.RequestPath} returned {(int)response.StatusCode} instead of 401. Body: {body}");
            }
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task PublicControllerEndpoints_Should_Not_RequireAuthenticationAsync()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services)
            .Where(endpoint => !endpoint.RequiresAuthorization)
            .ToArray();
        var failures = new List<string>();

        using var client = _factory.CreateAnonymousClient();
        foreach (var endpoint in endpoints)
        {
            using var request = CreateRequest(endpoint, Guid.NewGuid().ToString());
            using var response = await client.SendAsync(request);

            if (response.StatusCode is HttpStatusCode.Unauthorized)
            {
                failures.Add(
                    $"{endpoint.HttpMethod} {endpoint.RequestPath} unexpectedly required authentication.");
            }
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public void StateChangingEndpoints_Should_DeclareIdempotencyHeaderContract()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services)
            .Where(endpoint => endpoint.HttpMethod is "POST" or "PUT" or "PATCH" or "DELETE")
            .Where(RequiresIdempotencyHeaderContract)
            .ToArray();

        var missingContracts = endpoints
            .Where(endpoint => !HasIdempotencyHeaderParameter(endpoint.ActionDescriptor))
            .Select(endpoint => $"{endpoint.HttpMethod} {endpoint.RouteTemplate}")
            .ToArray();

        missingContracts.Should().BeEmpty();
    }

    [Fact]
    public async Task StateChangingEndpoints_Should_ReturnBadRequest_When_IdempotencyKeyIsInvalidAsync()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services)
            .Where(endpoint => endpoint.HttpMethod is "DELETE")
            .Where(endpoint => HasIdempotencyHeaderParameter(endpoint.ActionDescriptor))
            .ToArray();
        var failures = new List<string>();

        foreach (var endpoint in endpoints)
        {
            using var client = _factory.CreateAuthenticatedClient(
                $"idempotency-{Guid.NewGuid():N}");
            using var request = CreateRequest(endpoint, "not-a-guid");
            using var response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                failures.Add(
                    $"{endpoint.HttpMethod} {endpoint.RequestPath} returned {(int)response.StatusCode} instead of 400.");
            }
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task HealthEndpoint_Should_ReturnSuccess_When_TestDatabaseIsAvailableAsync()
    {
        using var client = _factory.CreateAnonymousClient();

        using var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static HttpRequestMessage CreateRequest(
        ControllerEndpoint endpoint,
        string idempotencyKey)
    {
        var request = new HttpRequestMessage(
            new HttpMethod(endpoint.HttpMethod),
            endpoint.RequestPath);

        if (endpoint.HttpMethod is "POST" or "PUT" or "PATCH" or "DELETE")
            request.Headers.Add("X-Idempotency-Key", idempotencyKey);

        if (endpoint.HttpMethod is "POST" or "PUT" or "PATCH")
            request.Content = JsonContent.Create(new { });

        return request;
    }

    private static bool HasIdempotencyHeaderParameter(ControllerActionDescriptor action) =>
        action.MethodInfo
            .GetParameters()
            .SelectMany(parameter => parameter.GetCustomAttributes(inherit: true))
            .OfType<FromHeaderAttribute>()
            .Any(attribute => string.Equals(
                attribute.Name,
                "X-Idempotency-Key",
                StringComparison.OrdinalIgnoreCase));

    private static bool RequiresIdempotencyHeaderContract(ControllerEndpoint endpoint)
    {
        return endpoint.HttpMethod == "DELETE" || HasIdempotencyHeaderParameter(endpoint.ActionDescriptor);
    }
}
