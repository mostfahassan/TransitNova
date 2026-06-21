using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Repository.TripRepository;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class TripRepositoryPhase2Tests
{
    [Fact]
    public async Task StartNewTrip_ShouldTrackTripAsAdded_WithoutImplicitSave()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture, persist: false);
        await new TripCommandRepository(NullLogger<TripCommandRepository>.Instance, fixture.Context)
            .StartNewTrip(trip, default);
        fixture.Context.Entry(trip).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task StartNewTrip_ShouldPersistTrip_AfterUnitOfWorkSave()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture, persist: false);
        await new TripCommandRepository(NullLogger<TripCommandRepository>.Instance, fixture.Context)
            .StartNewTrip(trip, default);
        await fixture.Context.SaveChangesAsync();
        (await fixture.Context.Trips.AsNoTracking().AnyAsync(x => x.Id == trip.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task GetTripByIdAsync_ShouldIncludeShipments_WhenTripExists()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await new TripQueryRepository(fixture.Context).GetTripByIdAsync(trip.Id, default);
        result!.Shipments.Should().ContainSingle();
    }

    [Fact]
    public async Task GetTripByIdAsync_ShouldReturnNull_WhenTripDoesNotExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new TripQueryRepository(fixture.Context).GetTripByIdAsync(Guid.NewGuid(), default)).Should().BeNull();
    }

    [Fact]
    public async Task GetTripsAsync_ShouldReturnEmptyList_WhenNoTripsExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new TripQueryRepository(fixture.Context).GetTripsAsync(default)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetTripsAsync_ShouldProjectCarrierWarehouseAndShipmentCount_WhenTripExists()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        var result = (await new TripQueryRepository(fixture.Context).GetTripsAsync(default)).Single();
        result.Id.Should().Be(trip.Id);
        result.TotalShipments.Should().Be(1);
        result.CarrierName.Should().Be("Omar One");
    }

    [Theory]
    [InlineData(TripType.Pickup, true)]
    [InlineData(TripType.Delivery, false)]
    public async Task GetTripByTypeAsync_ShouldFilterTrips_ByRequestedType(TripType type, bool expected)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await CreateTripAsync(fixture);
        var result = await new TripQueryRepository(fixture.Context).GetTripByTypeAsync(type, default);
        result.Any().Should().Be(expected);
    }

    [Theory]
    [InlineData(TripStatus.Planned, true)]
    [InlineData(TripStatus.Completed, false)]
    public async Task GetTripByStatusAsync_ShouldFilterTrips_ByRequestedStatus(TripStatus status, bool expected)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await CreateTripAsync(fixture);
        var result = await new TripQueryRepository(fixture.Context).GetTripByStatusAsync(status, default);
        result.Any().Should().Be(expected);
    }

    [Fact]
    public async Task GetTripByTypeAndIdAsync_ShouldReturnNull_WhenTypeDoesNotMatch()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        (await new TripQueryRepository(fixture.Context)
            .GetTripByTypeAsync(trip.Id, TripType.Delivery, default)).Should().BeNull();
    }

    [Fact]
    public async Task GetTripByStatusAndIdAsync_ShouldReturnProjection_WhenStatusMatches()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        (await new TripQueryRepository(fixture.Context)
            .GetTripByStatusAsync(trip.Id, TripStatus.Planned, default)).Should().NotBeNull();
    }

    [Fact]
    public async Task GetCarrierTripsAsync_ShouldReturnOnlyCarrierTrips_WhenCarrierMatches()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        var result = await new TripQueryRepository(fixture.Context).GetCarrierTripsAsync(trip.CarrierId, default);
        result.Should().ContainSingle(x => x.Id == trip.Id);
    }

    [Fact]
    public async Task GetCarrierTripsAsync_ShouldReturnEmptyList_WhenCarrierDoesNotMatch()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await CreateTripAsync(fixture);
        (await new TripQueryRepository(fixture.Context)
            .GetCarrierTripsAsync(Guid.NewGuid(), default)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetCarrierTripAsync_ShouldReturnDetails_WhenCarrierAndTripMatch()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        var result = await new TripQueryRepository(fixture.Context)
            .GetCarrierTripAsync(trip.CarrierId, trip.Id, default);
        result.Should().NotBeNull();
        result!.ShipmentCount.Should().Be(1);
    }

    private static async Task<Trip> CreateTripAsync(
        SqliteAppDbContextFixture fixture, bool persist = true)
    {
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var carrier = await Phase2RepositoryTestData.AddCarrierAsync(fixture, location.City);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        var warehouse = await Phase2RepositoryTestData.AddWarehouseAsync(fixture);
        shipment.ApproveShipment(Guid.NewGuid());
        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, carrier.Id, Guid.NewGuid());
        var trip = Trip.Plan(carrier.Id, warehouse.Id, TripType.Pickup, [shipment]);
        typeof(Shipment).GetProperty(nameof(Shipment.HandledById))!.SetValue(shipment, null);
        foreach (var entry in fixture.Context.ChangeTracker
                     .Entries<ShipmentStatus>()
                     .Where(x => x.State == EntityState.Added)
                     .ToList())
            entry.State = EntityState.Detached;
        await Phase2RepositoryTestData.PrepareTripAsync(fixture, trip);
        if (persist)
        {
            fixture.Context.Trips.Add(trip);
            await fixture.Context.SaveChangesAsync();
        }
        return trip;
    }
}
