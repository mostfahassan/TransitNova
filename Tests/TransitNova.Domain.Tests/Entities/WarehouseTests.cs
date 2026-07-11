using FluentAssertions;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.Domain.Tests.Entities;

public sealed class WarehouseTests
{
    private readonly Address _validAddress = Address.Create("Cairo", null, "Central Street");

    [Fact]
    public void CreateWarehouse_Should_Create_ActiveWarehouse_When_DataIsValid()
    {
        var userId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var address = Address.Create("Cairo", null, "Central Street");

        var warehouse = Warehouse.Create(" Main ", WarehouseType.MainWarehouse, 1000m, 100m, 24, address, userId, managerId);

        warehouse.Id.Should().NotBeEmpty();
        warehouse.Name.Should().Be("Main");
        warehouse.Capacity.Should().Be(1000m);
        warehouse.CurrentUsage.Should().Be(100m);
        warehouse.Address.Should().Be(address); // مقارنة الـ Record بالكامل
        warehouse.CreatedBy.Should().Be(userId.ToString());
        warehouse.CurrentState.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateWarehouse_Should_ThrowException_When_NameIsMissing(string invalidName)
    {
        var act = () => Warehouse.Create(invalidName, WarehouseType.MainWarehouse, 1000m, 100m, 24, _validAddress, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainArgumentException>();
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_AddressIsNull()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 1000m, 100m, 24, null!, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateWarehouse_Should_ThrowException_When_CapacityIsNotPositive(decimal capacity)
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, capacity, 0m, 8, _validAddress, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("WAREHOUSE_CAPACITY_INVALID");
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_UsageExceedsCapacity()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 100m, 101m, 8, _validAddress, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainOperationException>().Which.ErrorCode.Should().Be("WAREHOUSE_USAGE_EXCEEDS_CAPACITY");
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_UsageIsNegative()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 100m, -1m, 8, _validAddress, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("WAREHOUSE_USAGE_INVALID");
    }

    [Fact]
    public void CreateWarehouse_Should_ThrowException_When_OperatingHoursAreNotPositive()
    {
        var act = () => Warehouse.Create("Name", WarehouseType.MainWarehouse, 100m, 0m, 0, _validAddress, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("WAREHOUSE_OPERATING_HOURS_INVALID");
    }

    [Fact]
    public void UpdateWarehouse_Should_UpdateAndTrimValues_When_DataIsValid()
    {
        var warehouse = CreateWarehouse();
        var userId = Guid.NewGuid();
        var admin = Guid.NewGuid();
        var newAddress = Address.Create("New address", "Floor 2", "Updated Street");

        warehouse.Update(userId, " Updated ", WarehouseType.BranchWarehouse, 500m, 50m, 12, newAddress, admin);

        warehouse.Name.Should().Be("Updated");
        warehouse.Type.Should().Be(WarehouseType.BranchWarehouse);
        warehouse.Address.Should().Be(newAddress);
        warehouse.UpdatedBy.Should().Be(userId.ToString());
    }

    [Fact]
    public void AddZone_Should_AddOnlyOnce_When_SameZoneIsAddedTwice()
    {
        var warehouse = CreateWarehouse();
        var zone = Zone.Create("Zone", 1);

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
        var zone = Zone.Create("Zone", 1);
        warehouse.AddZone(zone);

        warehouse.RemoveZone(zone);

        warehouse.ZonesServed.Should().BeEmpty();
    }

    [Fact]
    public void ReplaceZones_Should_DeduplicateAndUpdateAuditFields_When_ZonesProvided()
    {
        var warehouse = CreateWarehouse();
        var first = Zone.Create("One", 1);
        var second = Zone.Create("Two", 1);
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
        Warehouse.Create("Main", WarehouseType.MainWarehouse, 1000m, 100m, 24, Address.Create("Cairo", null, "Central Street"), Guid.NewGuid(), Guid.NewGuid());
}