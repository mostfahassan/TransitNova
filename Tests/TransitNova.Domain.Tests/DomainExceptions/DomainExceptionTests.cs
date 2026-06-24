using FluentAssertions;
using TransitNova.Domain.DomainExceptions;

namespace TransitNova.Domain.Tests.DomainExceptions;

public sealed class DomainExceptionTests
{
    [Fact]
    public void DomainArgumentException_Should_PreserveStructuredContext_When_Constructed()
    {
        var entityId = Guid.NewGuid();

        var exception = new DomainArgumentException("name", "Name is required", "NAME_REQUIRED", "Warehouse", entityId);

        exception.Message.Should().Be("Name is required");
        exception.ParamName.Should().Be("name");
        exception.ErrorCode.Should().Be("NAME_REQUIRED");
        exception.EntityType.Should().Be("Warehouse");
        exception.EntityId.Should().Be(entityId);
    }

    [Fact]
    public void InvalidShipmentStateException_Should_DescribeExpectedAndActualState_When_Constructed()
    {
        var shipmentId = Guid.NewGuid();

        var exception = new InvalidShipmentStateException(shipmentId, "Pending", "Delivered");

        exception.ErrorCode.Should().Be("INVALID_SHIPMENT_STATE");
        exception.EntityType.Should().Be("Shipment");
        exception.EntityId.Should().Be(shipmentId);
        exception.Message.Should().Contain("Pending").And.Contain("Delivered");
    }

    [Fact]
    public void VehicleCapacityExceededException_Should_PreserveVehicleContext_When_Constructed()
    {
        var vehicleId = Guid.NewGuid();

        var exception = new VehicleCapacityExceededException(vehicleId, 120m, 30m);

        exception.ErrorCode.Should().Be("VEHICLE_CAPACITY_EXCEEDED");
        exception.EntityType.Should().Be("Vehicle");
        exception.EntityId.Should().Be(vehicleId);
        exception.Message.Should().Contain("120").And.Contain("30");
    }

    [Fact]
    public void ShipmentNotAssignedException_Should_PreserveShipmentContext_When_Constructed()
    {
        var shipmentId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();

        var exception = new ShipmentNotAssignedException(shipmentId, carrierId);

        exception.ErrorCode.Should().Be("SHIPMENT_NOT_ASSIGNED_EXCEPTION");
        exception.EntityId.Should().BeNull();
        exception.Message.Should().Contain(shipmentId.ToString());
        exception.Message.Should().Contain(carrierId.ToString());
    }
}
