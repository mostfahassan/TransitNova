using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TransitNova.Api.IntegrationTests.Infrastructure;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.Api.IntegrationTests;

public sealed class ApiSerializationContractTests : IClassFixture<TransitNovaWebApplicationFactory>, IAsyncLifetime
{
    private readonly TransitNovaWebApplicationFactory _factory;

    public ApiSerializationContractTests(TransitNovaWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => _factory.InitializeDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task BoundApiContractTypes_Should_MatchApprovedSerializationSnapshotsAsync()
    {
        var serializerOptions = _factory.Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        var contractTypes = ControllerEndpointCatalog.Discover(_factory.Services)
            .SelectMany(endpoint => endpoint.ActionDescriptor.Parameters.OfType<ControllerParameterDescriptor>())
            .Where(parameter => parameter.ParameterType != typeof(CancellationToken))
            .Where(parameter => parameter.ParameterType != typeof(System.Security.Claims.ClaimsPrincipal))
            .Select(parameter => Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType)
            .Where(type => type != typeof(string))
            .Where(type => !type.IsPrimitive)
            .Where(type => !type.IsEnum)
            .Where(type => type != typeof(Guid) && type != typeof(decimal) && type != typeof(DateTime) && type != typeof(DateOnly) && type != typeof(TimeOnly))
            .Distinct()
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        var snapshots = contractTypes
            .Select(type => CreateTypeSerializationSnapshot(type, serializerOptions))
            .ToArray();

        snapshots.Should().NotBeEmpty();
        await ApprovedJsonSnapshot.AssertMatchesAsync("ContractSnapshots/serialization-contract-types.json", snapshots);
    }

    [Fact]
    public async Task SerializerProbe_Should_MatchApprovedWireFormatSnapshotAsync()
    {
        var serializerOptions = _factory.Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
        var probe = new SerializerProbe
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            CreatedAt = new DateTime(2026, 7, 9, 11, 45, 0, DateTimeKind.Utc),
            ShipDate = new DateOnly(2026, 7, 10),
            DispatchTime = new TimeOnly(14, 5, 30),
            Price = 199.95m,
            UserType = UserType.User,
            Notes = null,
            Tags = [],
            Scores = [1, 2, 3]
        };

        var json = JsonSerializer.Serialize(probe, serializerOptions);
        var roundTrip = JsonSerializer.Deserialize<SerializerProbe>(json, serializerOptions);

        roundTrip.Should().NotBeNull();
        roundTrip!.Id.Should().Be(probe.Id);
        roundTrip.CreatedAt.Should().Be(probe.CreatedAt);
        roundTrip.ShipDate.Should().Be(probe.ShipDate);
        roundTrip.DispatchTime.Should().Be(probe.DispatchTime);
        roundTrip.Price.Should().Be(probe.Price);
        roundTrip.UserType.Should().Be(probe.UserType);
        roundTrip.Notes.Should().BeNull();
        roundTrip.Tags.Should().BeEmpty();
        roundTrip.Scores.Should().Equal(probe.Scores);

        var snapshot = new SerializerProbeContractSnapshot(
            nameof(SerializerProbe),
            json,
            JsonContractInspector.FlattenJsonPayload(json, "application/json"));

        await ApprovedJsonSnapshot.AssertMatchesAsync("ContractSnapshots/serializer-probe.json", snapshot);
    }

    private static SerializerProbeContractSnapshot CreateTypeSerializationSnapshot(Type type, JsonSerializerOptions serializerOptions)
    {
        var sample = SampleObjectFactory.Create(type);
        sample.Should().NotBeNull($"sample object generation should succeed for contract type {type.FullName}");

        var json = JsonSerializer.Serialize(sample, type, serializerOptions);

        return new SerializerProbeContractSnapshot(
            type.FullName ?? type.Name,
            json,
            JsonContractInspector.FlattenJsonPayload(json, "application/json"));
    }

    private sealed class SerializerProbe
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateOnly ShipDate { get; init; }
        public TimeOnly DispatchTime { get; init; }
        public decimal Price { get; init; }
        public UserType UserType { get; init; }
        public string? Notes { get; init; }
        public List<string> Tags { get; init; } = [];
        public List<int> Scores { get; init; } = [];
    }
}
