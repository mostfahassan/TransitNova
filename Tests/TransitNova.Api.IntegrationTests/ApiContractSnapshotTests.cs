using FluentAssertions;
using TransitNova.Api.IntegrationTests.Infrastructure;

namespace TransitNova.Api.IntegrationTests;

public sealed class ApiContractSnapshotTests : IClassFixture<TransitNovaWebApplicationFactory>, IAsyncLifetime
{
    private readonly TransitNovaWebApplicationFactory _factory;

    public ApiContractSnapshotTests(TransitNovaWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => _factory.InitializeDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task EndpointResponses_Should_MatchApprovedSnapshotAsync()
    {
        var endpoints = ControllerEndpointCatalog.Discover(_factory.Services);
        var snapshots = new List<EndpointResponseContractSnapshot>();

        foreach (var endpoint in endpoints)
        {
            using var client = endpoint.RequiresAuthorization
                ? _factory.CreateAuthenticatedClient($"contract-{Guid.NewGuid():N}")
                : _factory.CreateAnonymousClient();
            using var request = await EndpointRequestFactory.CreateRequestAsync(_factory, endpoint, Guid.NewGuid().ToString());
            using var response = await client.SendAsync(request);

            snapshots.Add(await JsonContractInspector.CreateEndpointResponseSnapshotAsync(endpoint, response));
        }

        snapshots.Should().HaveCount(ControllerEndpointCatalogSnapshot.ExpectedEndpointCount);
        await ApprovedJsonSnapshot.AssertMatchesAsync("ContractSnapshots/endpoint-response-contracts.json", snapshots);
    }

    [Fact]
    public async Task EndpointRequestContracts_Should_MatchApprovedSnapshotAsync()
    {
        await _factory.InitializeDatabaseAsync();
        var snapshots = JsonContractInspector.CreateRequestContractSnapshots(_factory.Services);

        snapshots.Should().HaveCount(ControllerEndpointCatalogSnapshot.ExpectedEndpointCount);
        await ApprovedJsonSnapshot.AssertMatchesAsync("ContractSnapshots/endpoint-request-contracts.json", snapshots);
    }
}
