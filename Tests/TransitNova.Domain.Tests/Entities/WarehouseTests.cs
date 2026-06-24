using FluentAssertions;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.Domain.Tests.Entities;

public sealed class WarehouseTests
{
    [Fact]
    public void CreateWarehouse_Should_Create_ActiveWarehouse_When_DataIsValid()
    {
        var userId = Guid.NewGuid();

        var warehouse = Warehouse.Create(" Main ", WarehouseType.MainWarehouse, 1000m, 100m, 24, "Cairo", userId);

        warehouse.Id.Should().NotBeEmpty();
        warehouse.Name.Should().Be("Main");
        warehouse.Capacity.Should().Be(1000m);
        warehouse.CurrentUsage.Should().Be(100m);
        warehouse.CreatedBy.Should().Be(userId.ToString());
        warehouse.CurrentState.Should().BeTrue();
    }

    [Theory]
    [InlineData("", 100, 0, 8, "Address")]
    [InlineData("Name", 100, 0, 8, "")]
    public void CreateWarehouse_Should_ThrowException_When_RequiredTextIsMissing(string name, decimal capacity, decimal usage, int hours, string address)
    {
        var act = () => Warehouse.Create(name, WarehouseType.MainWarehouse, capacity, usage, hours, address, Guid.NewGuid());

        act.Should().Throw<DomainArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateWarehouse_Should_ThrowException_When_CapacityIsNotPositive(decimal capacity)
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, capacity, 0m, 8, "Address", Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("WAREHOUSE_CAPACITY_INVALID");
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_UsageExceedsCapacity()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 100m, 101m, 8, "Address", Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("WAREHOUSE_USAGE_EXCEEDS_CAPACITY");
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_UsageIsNegative()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 100m, -1m, 8, "Address", Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("WAREHOUSE_USAGE_INVALID");
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_OperatingHoursAreNotPositive()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 100m, 0m, 0, "Address", Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("WAREHOUSE_OPERATING_HOURS_INVALID");
    }

    [Fact]
    public void UpdateWarehouse_Should_UpdateAndTrimValues_When_DataIsValid()
    {
        var warehouse = CreateWarehouse();
        var userId = Guid.NewGuid();

        warehouse.Update(" Updated ", WarehouseType.BranchWarehouse, 500m, 50m, 12, " New address ", userId);

        warehouse.Name.Should().Be("Updated");
        warehouse.Type.Should().Be(WarehouseType.BranchWarehouse);
        warehouse.Address.Should().Be("New address");
        warehouse.UpdatedBy.Should().Be(userId.ToString());
    }

    [Fact]
    public void AddZone_Should_AddOnlyOnce_When_SameZoneIsAddedTwice()
    {
        var warehouse = CreateWarehouse();
        var zone = Zone.Create("Zone", "Z1", 1);

        warehouse.AddZone(zone);
        warehouse.AddZone(zone);

        warehouse.ZonesServed.Should().ContainSingle().Which.Should().Be(zone);
    }

    [Fact]
    public void AddZone_Should_ThrowException_When_ZoneIsNull()
    {
        var warehouse = CreateWarehouse();

        var act = () => warehouse.AddZone(null!);

        act.Should().Throw<DomainArgumentException>().Which.ErrorCode.Should().Be("ZONE_REQUIRED");
    }

    [Fact]
    public void RemoveZone_Should_RemoveExistingZone_When_ZoneExists()
    {
        var warehouse = CreateWarehouse();
        var zone = Zone.Create("Zone", "Z1", 1);
        warehouse.AddZone(zone);

        warehouse.RemoveZone(zone);

        warehouse.ZonesServed.Should().BeEmpty();
    }

    [Fact]
    public void ReplaceZones_Should_DeduplicateAndUpdateAuditFields_When_ZonesProvided()
    {
        var warehouse = CreateWarehouse();
        var first = Zone.Create("One", "Z1", 1);
        var second = Zone.Create("Two", "Z2", 1);
        var userId = Guid.NewGuid();

        warehouse.ReplaceZones([first, first, second], userId);

        warehouse.ZonesServed.Should().BeEquivalentTo([first, second]);
        warehouse.UpdatedBy.Should().Be(userId.ToString());
    }

    [Fact]
    public void ReplaceZones_Should_ThrowException_When_ZonesAreNull()
    {
        var warehouse = CreateWarehouse();

        var act = () => warehouse.ReplaceZones(null!, Guid.NewGuid());

        act.Should().Throw<DomainArgumentException>().Which.ErrorCode.Should().Be("ZONES_REQUIRED");
    }

    private static Warehouse CreateWarehouse() =>
        Warehouse.Create("Main", WarehouseType.MainWarehouse, 1000m, 100m, 24, "Cairo", Guid.NewGuid());
}
