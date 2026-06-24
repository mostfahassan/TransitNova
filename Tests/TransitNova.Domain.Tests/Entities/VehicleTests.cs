using FluentAssertions;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;

namespace TransitNova.Domain.Tests.Entities;

public sealed class VehicleTests
{
    [Fact]
    public void CreateVehicle_Should_Create_ActiveVehicle_When_DataIsValid()
    {
        var carrierId = Guid.NewGuid();

        var vehicle = Vehicle.Create(VehicleType.Truck, " ABC-123 ", 1000m, 50m, true, carrierId);

        vehicle.Id.Should().NotBeEmpty();
        vehicle.VehicleType.Should().Be(VehicleType.Truck);
        vehicle.PlateNumber.Should().Be("ABC-123");
        vehicle.CapacityWeight.Should().Be(1000m);
        vehicle.CapacityVolume.Should().Be(50m);
        vehicle.IsRefrigerated.Should().BeTrue();
        vehicle.IsActive.Should().BeTrue();
        vehicle.CarrierId.Should().Be(carrierId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateVehicle_Should_ThrowException_When_WeightIsNotPositive(decimal weight)
    {
        var act = () => Vehicle.Create(VehicleType.Truck, "ABC", weight, 10m, false, Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ParamName.Should().Be("capacityWeight");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateVehicle_Should_ThrowException_When_VolumeIsNotPositive(decimal volume)
    {
        var act = () => Vehicle.Create(VehicleType.Truck, "ABC", 10m, volume, false, Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ParamName.Should().Be("capacityVolume");
    }

    [Fact]
    public void CreateVehicle_Should_ThrowException_When_PlateNumberIsMissing()
    {
        var act = () => Vehicle.Create(VehicleType.Truck, " ", 10m, 10m, false, Guid.NewGuid());

        act.Should().Throw<DomainArgumentException>().Which.ErrorCode.Should().Be("ARG_PLATENUMBER_REQUIRED");
    }

    [Fact]
    public void CreateVehicle_Should_ThrowException_When_PlateNumberExceedsMaximumLength()
    {
        var act = () => Vehicle.Create(VehicleType.Truck, new string('A', 51), 10m, 10m, false, Guid.NewGuid());

        act.Should().Throw<DomainArgumentException>().Which.ErrorCode.Should().Be("ARG_PLATENUMBER_LENGTH");
    }

    [Fact]
    public void CreateVehicle_Should_ThrowException_When_VehicleTypeIsUndefined()
    {
        var act = () => Vehicle.Create((VehicleType)999, "ABC", 10m, 10m, false, Guid.NewGuid());

        act.Should().Throw<DomainArgumentOutOfRangeException>().Which.ErrorCode.Should().Be("ARG_VEHICLETYPE_OUT_OF_RANGE");
    }

    [Fact]
    public void UpdateVehicle_Should_Update_AllMutableFields_When_DataIsValid()
    {
        var vehicle = CreateVehicle();

        vehicle.UpdateVehicle(VehicleType.Van, "NEW-1", 500m, 20m, true);

        vehicle.VehicleType.Should().Be(VehicleType.Van);
        vehicle.PlateNumber.Should().Be("NEW-1");
        vehicle.CapacityWeight.Should().Be(500m);
        vehicle.CapacityVolume.Should().Be(20m);
        vehicle.IsRefrigerated.Should().BeTrue();
        vehicle.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void DeactivateAndActivate_Should_ChangeAvailability_When_Called()
    {
        var vehicle = CreateVehicle();

        vehicle.Deactivate();
        vehicle.IsActive.Should().BeFalse();
        vehicle.Activate();

        vehicle.IsActive.Should().BeTrue();
        vehicle.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ChangeCarrier_Should_ChangeCarrier_When_IdIsValid()
    {
        var vehicle = CreateVehicle();
        var carrierId = Guid.NewGuid();

        vehicle.ChangeCarrier(carrierId);

        vehicle.CarrierId.Should().Be(carrierId);
    }

    [Fact]
    public void ChangeCarrier_Should_ThrowException_When_IdIsEmpty()
    {
        var vehicle = CreateVehicle();

        var act = () => vehicle.ChangeCarrier(Guid.Empty);

        act.Should().Throw<DomainArgumentException>().Which.ErrorCode.Should().Be("ARG_CARRIERID_EMPTY");
    }

    private static Vehicle CreateVehicle() =>
        Vehicle.Create(VehicleType.Truck, "ABC-123", 1000m, 50m, false, Guid.NewGuid());
}
