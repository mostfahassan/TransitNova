using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.InfraStructure.Repository.CarrierRepo;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class CarrierRepositoryPhase2Tests
{
    [Fact]
    public async Task GetCarrierAsync_ShouldReturnCarrier_WhenPredicateMatchesAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        (await CreateRepository(fixture).GetCarrierAsync(x => x.Id == carrier.Id, default))
            .Should().BeSameAs(carrier);
    }

    [Fact]
    public async Task GetCarrierAsync_ShouldReturnNull_WhenPredicateDoesNotMatchAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var missingId = Guid.NewGuid();
        (await CreateRepository(fixture).GetCarrierAsync(x => x.Id == missingId, default)).Should().BeNull();
    }

    [Fact]
    public async Task GetCarrierByAppUserIdAsync_ShouldReturnUntrackedCarrier_ByDefaultAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await CreateRepository(fixture).GetCarrierByAppUserIdAsync(carrier.AppUserId);
        result!.Id.Should().Be(carrier.Id);
        fixture.Context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task GetCarrierByAppUserIdAsync_ShouldTrackCarrier_WhenTrackedIsTrueAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await CreateRepository(fixture).GetCarrierByAppUserIdAsync(carrier.AppUserId, true);
        fixture.Context.Entry(result!).State.Should().Be(EntityState.Unchanged);
    }

    [Fact]
    public async Task GetCarrierByAppUserIdAsync_ShouldReturnNull_WhenCarrierDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateRepository(fixture).GetCarrierByAppUserIdAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task GetCarrierNameAsync_ShouldReturnFullName_WhenCarrierExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        (await fixture.Context.Carriers.IgnoreQueryFilters().CountAsync()).Should().Be(1);
        (await CreateRepository(fixture).GetCarrierNameAsync(carrier.Id)).Should().Be("Omar One");
    }

    [Fact]
    public async Task GetCarrierNameAsync_ShouldReturnNull_WhenCarrierDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateRepository(fixture).GetCarrierNameAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnAvailable_WhenNewCarrierExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        (await CreateRepository(fixture).GetStatusAsync(carrier.Id)).Should().Be(CarrierStatus.Available);
    }

    [Fact]
    public async Task GetCarrierByAppUserIdAsync_ShouldReturnCarrier_WhenAppUserIdExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        var result = await CreateRepository(fixture).GetCarrierByAppUserIdAsync(carrier.AppUserId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(carrier.Id);
    }

    [Fact]
    public async Task GetCarrierByAppUserIdAsync_ShouldReturnNull_WhenAppUserIdDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateRepository(fixture).GetCarrierByAppUserIdAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task GetCarrierForTripAsync_ShouldLoadTripsCollection_WhenCarrierExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await CreateRepository(fixture).GetCarrierForTripAsync(x => x.Id == carrier.Id);
        result.Should().NotBeNull();
        result!.Trips.Should().BeEmpty();
    }

    [Fact]
    public async Task FilterByCriteriaAsync_ShouldReturnEmptyPage_WhenNoCarriersExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var result = await CreateRepository(fixture)
            .FilterByCriteriaAsync<CarrierProfileDto>(new FilterCarrierDto(), false, default);
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAverageRatingAsync_ShouldReturnPersistedAverage_WhenCarrierExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var carrier = await AddCarrierAsync(fixture);
        carrier.AddRating(4);
        await fixture.Context.SaveChangesAsync();
        (await new CarrierAnalyticsQueryRepository(fixture.Context)
            .GetAverageRatingAsync(carrier.Id)).Should().Be(4m);
    }

    [Fact]
    public async Task GetCarrierRevenueAsync_ShouldReturnZero_WhenCarrierHasNoDeliveredShipmentsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new CarrierAnalyticsQueryRepository(fixture.Context)
            .GetCarrierRevenueAsync(Guid.NewGuid())).Should().Be(0m);
    }

    [Fact]
    public async Task GetCarrierShipmentAsync_ShouldReturnNull_WhenNoAssignmentExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateShipmentRepository(fixture).GetCarrierShipmentAsync(
            Guid.NewGuid(), Guid.NewGuid())).Should().BeNull();
    }

  

    [Fact]
    public async Task GetCarrierShipmentCountInStatus_ShouldReturnEmptyDictionary_WhenNoAssignmentsExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateShipmentRepository(fixture)
            .GetCarrierShipmentCountInStatusAsync(Guid.NewGuid())).Should().BeEmpty();
    }

    private static CarrierQueryRepository CreateRepository(SqliteAppDbContextFixture fixture) =>
        new(Phase2RepositoryTestData.CreateMapper(), fixture.Context);

    private static CarrierShipmentQueryRepository CreateShipmentRepository(SqliteAppDbContextFixture fixture) =>
        new(fixture.Context, Phase2RepositoryTestData.CreateMapper(),
            NullLogger<CarrierShipmentQueryRepository>.Instance);

    private static async Task<TransitNova.Domain.Entities.MainEntities.Carrier> AddCarrierAsync(
        SqliteAppDbContextFixture fixture)
    {
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        return await Phase2RepositoryTestData.AddCarrierAsync(fixture, location.City);
    }
}
