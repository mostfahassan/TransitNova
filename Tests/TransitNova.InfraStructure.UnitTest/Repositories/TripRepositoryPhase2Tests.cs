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
    public async Task StartNewTrip_ShouldTrackTripAsAdded_WithoutImplicitSaveAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture, persist: false);
        await new TripCommandRepository(NullLogger<TripCommandRepository>.Instance, fixture.Context)
            .StartNewTripAsync(trip, default);
        fixture.Context.Entry(trip).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task StartNewTrip_ShouldPersistTrip_AfterUnitOfWorkSaveAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture, persist: false);
        await new TripCommandRepository(NullLogger<TripCommandRepository>.Instance, fixture.Context)
            .StartNewTripAsync(trip, default);
        await fixture.Context.SaveChangesAsync();
        (await fixture.Context.Trips.AsNoTracking().AnyAsync(x => x.Id == trip.Id)).Should().BeTrue();
    }

 

    [Fact]
    public async Task GetCarrierTripsAsync_ShouldReturnOnlyCarrierTrips_WhenCarrierMatchesAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var trip = await CreateTripAsync(fixture);
        var result = await new TripQueryRepository(fixture.Context).GetCarrierTripsAsync(trip.CarrierId, default);
        result.Should().ContainSingle(x => x.Id == trip.Id);
    }

    [Fact]
    public async Task GetCarrierTripsAsync_ShouldReturnEmptyList_WhenCarrierDoesNotMatchAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await CreateTripAsync(fixture);
        (await new TripQueryRepository(fixture.Context)
            .GetCarrierTripsAsync(Guid.NewGuid(), default)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetCarrierTripAsync_ShouldReturnDetails_WhenCarrierAndTripMatchAsync()
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
        typeof(Shipment).GetProperty(nameof(Shipment.HandlerId))!.SetValue(shipment, null);
        foreach (var entry in fixture.Context.ChangeTracker
                     .Entries<ShipmentStatus>()
                     .Where(x => x.State == EntityState.Added)
                     .ToList())
            entry.State = EntityState.Detached;
        await Phase2RepositoryTestData.PrepareTripAsync(fixture);
        if (persist)
        {
            fixture.Context.Trips.Add(trip);
            await fixture.Context.SaveChangesAsync();
        }
        return trip;
    }
}
