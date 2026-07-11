using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net.Http.Json;
using TransitNova.Api.IntegrationTests.Infrastructure;
using TransitNova.Domain.Enums.Bundle;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.Api.IntegrationTests;

public sealed class ApiErrorContractTests : IClassFixture<TransitNovaWebApplicationFactory>, IAsyncLifetime
{
    private readonly TransitNovaWebApplicationFactory _factory;

    public ApiErrorContractTests(TransitNovaWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => _factory.InitializeDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task StandardErrorScenarios_Should_MatchApprovedSnapshotsAsync()
    {
        var snapshots = new List<ErrorScenarioContractSnapshot>
        {
            await CaptureInvalidIdempotencyScenarioAsync(),
            await CaptureUnauthorizedScenarioAsync(),
            await CaptureForbiddenScenarioAsync(),
            await CaptureNotFoundScenarioAsync(),
            await CaptureConflictScenarioAsync(),
            await CaptureValidationScenarioAsync()
        };

        snapshots.Select(snapshot => snapshot.StatusCode)
            .Should()
            .Contain([400, 401, 403, 404, 409, 422]);

        await ApprovedJsonSnapshot.AssertMatchesAsync("ContractSnapshots/error-response-contracts.json", snapshots);
    }

    private async Task<ErrorScenarioContractSnapshot> CaptureInvalidIdempotencyScenarioAsync()
    {
        var endpoint = ControllerEndpointCatalog.Discover(_factory.Services)
            .Where(endpoint => endpoint.HttpMethod == "DELETE")
            .First(RequiresIdempotencyHeaderContract);

        using var client = _factory.CreateAuthenticatedClient();
        using var request = await EndpointRequestFactory.CreateRequestAsync(_factory, endpoint, "not-a-guid");
        using var response = await client.SendAsync(request);

        return await CreateScenarioSnapshotAsync("BadRequest_InvalidIdempotencyKey", endpoint.RequestPath, response);
    }

    private async Task<ErrorScenarioContractSnapshot> CaptureUnauthorizedScenarioAsync()
    {
        using var client = _factory.CreateAnonymousClient();
        using var response = await client.GetAsync("/api/v1/warehouse-managers/dashboard");

        return await CreateScenarioSnapshotAsync("Unauthorized_AnonymousProtectedEndpoint", "/api/v1/warehouse-managers/dashboard", response);
    }

    private async Task<ErrorScenarioContractSnapshot> CaptureForbiddenScenarioAsync()
    {
        using var client = _factory.CreateAuthenticatedClient(
            roles: ["User"],
            permissions: [],
            bypassAuthorization: false);
        using var response = await client.GetAsync("/api/v1/warehouse-managers/dashboard");

        return await CreateScenarioSnapshotAsync("Forbidden_WarehouseManagerDashboard", "/api/v1/warehouse-managers/dashboard", response);
    }

    private async Task<ErrorScenarioContractSnapshot> CaptureNotFoundScenarioAsync()
    {
        using var client = _factory.CreateAnonymousClient();
        using var response = await client.GetAsync("/api/v1/governments/999999");

        return await CreateScenarioSnapshotAsync("NotFound_Government", "/api/v1/governments/999999", response);
    }

    private async Task<ErrorScenarioContractSnapshot> CaptureConflictScenarioAsync()
    {
        var requestId = Guid.NewGuid().ToString();
        var firstPayload = CreateBundlePayload("Starter Bundle", 199.99m);
        var secondPayload = CreateBundlePayload("Starter Bundle Updated", 249.99m);

        using var firstClient = _factory.CreateAuthenticatedClient();
        using var firstRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin/bundles")
        {
            Content = JsonContent.Create(firstPayload)
        };
        firstRequest.Headers.Add("X-Idempotency-Key", requestId);
        using var firstResponse = await firstClient.SendAsync(firstRequest);
        firstResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        using var secondClient = _factory.CreateAuthenticatedClient();
        using var secondRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin/bundles")
        {
            Content = JsonContent.Create(secondPayload)
        };
        secondRequest.Headers.Add("X-Idempotency-Key", requestId);
        using var secondResponse = await secondClient.SendAsync(secondRequest);

        return await CreateScenarioSnapshotAsync("Conflict_IdempotencyHashMismatch", "/api/v1/admin/bundles", secondResponse);
    }

    private async Task<ErrorScenarioContractSnapshot> CaptureValidationScenarioAsync()
    {
        using var client = _factory.CreateAuthenticatedClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin/warehouses")
        {
            Content = JsonContent.Create(CreateInvalidWarehousePayload())
        };
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        using var response = await client.SendAsync(request);

        return await CreateScenarioSnapshotAsync("Validation_CreateWarehouse_InvalidZones", "/api/v1/admin/warehouses", response);
    }

    private static object CreateInvalidWarehousePayload()
    {
        return new
        {
            Name = "Validation Warehouse",
            Type = WarehouseType.BranchWarehouse,
            Address = "Contract Address",
            Capacity = 1000m,
            CurrentUsage = 0m,
            OperatingHours = 24,
            ZoneIds = new[] { Guid.Parse("55555555-5555-5555-5555-555555555555") },
            ManagerId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };
    }

    private static object CreateBundlePayload(string bundleName, decimal bundlePrice)
    {
        return new
        {
            BundleName = bundleName,
            BundleDescription = "Contract snapshot bundle for idempotency mismatch testing.",
            BundlePrice = bundlePrice,
            Tier = BundleTier.Standard,
            BundleDurationMonths = 6,
            MaxShipmentsPerMonth = 10,
            MaxWeightPerShipment = 25m,
            MaxDistancePerShipment = 150m,
            DiscountPercentage = 5m,
            MinimumShipmentValueForDiscount = 100m
        };
    }

    private static async Task<ErrorScenarioContractSnapshot> CreateScenarioSnapshotAsync(
        string scenario,
        string endpoint,
        HttpResponseMessage response)
    {
        var content = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync();
        var contentType = response.Content?.Headers.ContentType?.ToString();
        var mediaType = response.Content?.Headers.ContentType?.MediaType;

        return new ErrorScenarioContractSnapshot(
            scenario,
            endpoint,
            (int)response.StatusCode,
            contentType,
            JsonContractInspector.FlattenJsonPayload(content, mediaType));
    }

    private static bool RequiresIdempotencyHeaderContract(ControllerEndpoint endpoint)
    {
        return endpoint.HttpMethod == "DELETE" || HasIdempotencyHeaderParameter(endpoint.ActionDescriptor);
    }

    private static bool HasIdempotencyHeaderParameter(ControllerActionDescriptor action)
    {
        return action.MethodInfo
            .GetParameters()
            .SelectMany(parameter => parameter.GetCustomAttributes(inherit: true))
            .OfType<FromHeaderAttribute>()
            .Any(attribute => string.Equals(
                attribute.Name,
                "X-Idempotency-Key",
                StringComparison.OrdinalIgnoreCase));
    }
}


