using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Repository.ShipmentRepo;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class ShipmentRepositoryPhase2Tests
{
    [Fact]
    public async Task AddAsync_ShouldTrackShipmentAsAdded_WithoutImplicitSave()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        fixture.Context.Shipments.Remove(shipment);
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();
        await new ShipmentCommandRepository(fixture.Context, NullLogger<ShipmentCommandRepository>.Instance)
            .AddAsync(shipment, default);
        fixture.Context.Entry(shipment).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistShipment_AfterUnitOfWorkSave()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        fixture.Context.ChangeTracker.Clear();
        (await fixture.Context.Shipments.AsNoTracking().AnyAsync(x => x.Id == shipment.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ShouldMarkDetachedShipmentAsModified_WhenCalled()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        fixture.Context.ChangeTracker.Clear();
        shipment.ApproveShipment(Guid.NewGuid());
        await new ShipmentCommandRepository(fixture.Context, NullLogger<ShipmentCommandRepository>.Instance)
            .UpdateAsync(shipment, default);
        fixture.Context.Entry(shipment).State.Should().Be(EntityState.Modified);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Exists_ShouldReturnExpectedResult_WhenShipmentMayExist(bool exists)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var id = Guid.NewGuid();
        if (exists)
        {
            var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
            var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
            id = (await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City)).Id;
        }
        (await new ShipmentRulesRepository(fixture.Context).Exists(id, default)).Should().Be(exists);
    }

    [Theory]
    [InlineData(ShipmentStatuses.Pending, true)]
    [InlineData(ShipmentStatuses.Approved, false)]
    public async Task Editable_ShouldMatchAllowedCurrentStatus_WhenShipmentExists(
        ShipmentStatuses allowedStatus, bool expected)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        var result = await new ShipmentRulesRepository(fixture.Context)
            .Editable(shipment.Id, [allowedStatus], default);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetByTrackingNumberAsync_ShouldReturnSummary_WhenTrackingNumberMatches()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var seeded = await AddShipmentGraphAsync(fixture);
        var result = await CreateQueryRepository(fixture)
            .GetByTrackingNumberAsync(seeded.Shipment.TrackingNumber, default);
        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Shipment.Id);
        result.SenderCity.Should().Be(seeded.Location.City.Name);
    }

    [Fact]
    public async Task GetByTrackingNumberAsync_ShouldReturnNull_WhenTrackingNumberDoesNotMatch()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateQueryRepository(fixture).GetByTrackingNumberAsync("missing", default)).Should().BeNull();
    }

    [Fact]
    public async Task GetShipmentCountInStatus_ShouldGroupPersistedRows_ByCurrentStatus()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        await AddShipmentGraphAsync(fixture);
        var result = await CreateQueryRepository(fixture).GetShipmentCountInStatus(default);
        result.Should().ContainKey(ShipmentStatuses.Pending).WhoseValue.Should().Be(1);
    }

    [Theory]
    [InlineData(ShipmentStatuses.Pending, true)]
    [InlineData(ShipmentStatuses.Delivered, false)]
    public async Task GetShipmentInStatusAsync_ShouldReturnOnlyMatchingShipment(
        ShipmentStatuses status, bool expected)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var seeded = await AddShipmentGraphAsync(fixture);
        var result = await CreateQueryRepository(fixture)
            .GetShipmentInStatusAsync(seeded.Shipment.Id, status, default);
        (result is not null).Should().Be(expected);
    }

    [Fact]
    public async Task GetShipmentInStatusAsync_ShouldIncludeSenderAndReceiver_WhenIncludesRequested()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var seeded = await AddShipmentGraphAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await CreateQueryRepository(fixture)
            .GetShipmentInStatusAsync(seeded.Shipment.Id, ShipmentStatuses.Pending, default, true);
        result!.Sender.Should().NotBeNull();
        result.Receiver.Should().NotBeNull();
    }

    [Fact]
    public async Task GetShipmentForCommands_ShouldReturnTrackedEntity_WhenShipmentExists()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var seeded = await AddShipmentGraphAsync(fixture);
        fixture.Context.ChangeTracker.Clear();
        var result = await CreateQueryRepository(fixture).GetShipmentForCommands(seeded.Shipment.Id, default);
        result.Should().NotBeNull();
        fixture.Context.Entry(result!).State.Should().Be(EntityState.Unchanged);
    }

    [Fact]
    public async Task GetEntityAsync_ShouldReturnNull_WhenShipmentDoesNotExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateQueryRepository(fixture).GetEntityAsync(Guid.NewGuid(), default)).Should().BeNull();
    }

    [Fact]
    public async Task GetShipmentHistoriesAsync_ShouldReturnInitialHistory_WhenShipmentWasCreated()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var seeded = await AddShipmentGraphAsync(fixture);
        var result = await CreateQueryRepository(fixture).GetShipmentHistoriesAsync(seeded.Shipment.Id, default);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetShipmentsAssignedToCarrierAsync_ShouldReturnEmpty_WhenCarrierHasNoAssignment()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var result = await CreateQueryRepository(fixture)
            .GetShipmentsAssignedToCarrierAsync(ShipmentStatuses.AssignedToPickUpCarrier, Guid.NewGuid(), default);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CanRateDeliveryCarrierAsync_ShouldReturnFalse_WhenShipmentDoesNotExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new ShipmentRulesRepository(fixture.Context).CanRateDeliveryCarrierAsync(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), default)).Should().BeFalse();
    }

    [Fact]
    public async Task CanRatePickUpCarrierAsync_ShouldReturnFalse_WhenShipmentDoesNotExist()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new ShipmentRulesRepository(fixture.Context).CanRatePickUpCarrierAsync(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), default)).Should().BeFalse();
    }

    private static ShipmentQueryRepository CreateQueryRepository(SqliteAppDbContextFixture fixture) =>
        new(fixture.Context, Phase2RepositoryTestData.CreateMapper(), NullLogger<ShipmentQueryRepository>.Instance);

    private static async Task<(TransitNova.Domain.Entities.MainEntities.Shipment Shipment,
        (TransitNova.Domain.Entities.MainEntities.Country Country,
         TransitNova.Domain.Entities.MainEntities.Government Government,
         TransitNova.Domain.Entities.MainEntities.City City) Location)> AddShipmentGraphAsync(
        SqliteAppDbContextFixture fixture)
    {
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        return (shipment, location);
    }
}
