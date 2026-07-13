using FluentAssertions;
using FluentValidation;
using Moq;
using TransitNova.ApplicationLayer.Tests.TestData;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Features.Vehicles.Commands;
using TransitNova.BusinessLayer.Features.Vehicles.Commands.CommandsValidators;
using TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository;
using TransitNova.BusinessLayer.Validators.ShipmentValidators;
using TransitNova.BusinessLayer.Validators.TripValidators;
using TransitNova.BusinessLayer.Validators.VehicleValidators;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.ApplicationLayer.Tests.Validators;

public sealed class HighRiskValidatorCoverageTests
{
    [Fact]
    public async Task ShipmentValidator_Should_ValidateAggregateRulesAsync()
    {
        var shipment = ShipmentTestData.CreateShipment();

        var result = await new ShipmentValidator().ValidateAsync(shipment);

        result.Errors.Should().Contain(error => error.PropertyName == "ShipmentCost");
        result.Errors.Should().Contain(error => error.PropertyName == "Sender");
        result.Errors.Should().NotContain(error => error.PropertyName == "DeliveryAddress");
        result.Errors.Should().NotContain(error => error.PropertyName == "PickupAddress");
    }

    [Fact]
    public async Task ShipmentFilterValidator_Should_AcceptValidFilterAsync()
    {
        var filter = new ShipmentFilterDto
        {
            Status = [ShipmentStatuses.Approved, ShipmentStatuses.InWarehouse],
            Mode = TransportationMode.Land,
            From = DateTime.UtcNow.AddDays(-2),
            To = DateTime.UtcNow.AddDays(-1),
            SenderId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 30
        };

        var result = await new ShipmentFilterValidator().ValidateAsync(filter);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("page")]
    [InlineData("pageSize")]
    [InlineData("statusCount")]
    [InlineData("statusInvalid")]
    [InlineData("statusDuplicate")]
    [InlineData("mode")]
    [InlineData("fromFuture")]
    [InlineData("toFuture")]
    [InlineData("range")]
    [InlineData("sender")]
    public async Task ShipmentFilterValidator_Should_RejectInvalidConstraintsAsync(string scenario)
    {
        var filter = new ShipmentFilterDto();
        switch (scenario)
        {
            case "page": filter.PageNumber = 0; break;
            case "pageSize": filter.PageSize = 31; break;
            case "statusCount": filter.Status = Enumerable.Repeat(ShipmentStatuses.Approved, 6).ToArray(); break;
            case "statusInvalid": filter.Status = [(ShipmentStatuses)999]; break;
            case "statusDuplicate": filter.Status = [ShipmentStatuses.Approved, ShipmentStatuses.Approved]; break;
            case "mode": filter.Mode = (TransportationMode)999; break;
            case "fromFuture": filter.From = DateTime.UtcNow.AddDays(1); break;
            case "toFuture": filter.To = DateTime.UtcNow.AddDays(1); break;
            case "range": filter.From = DateTime.UtcNow.AddDays(-1); filter.To = DateTime.UtcNow.AddDays(-2); break;
            case "sender": filter.SenderId = Guid.Empty; break;
        }

        var result = await new ShipmentFilterValidator().ValidateAsync(filter);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task FilterTripsDtoValidator_Should_AcceptValidFilterAsync()
    {
        var filter = new FilterTripsDto
        {
            Id = Guid.NewGuid(),
            TripType = TripType.Pickup,
            Status = [TripStatus.Planned, TripStatus.Active],
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            From = DateTime.UtcNow.AddDays(-2),
            To = DateTime.UtcNow.AddDays(-1),
            CarrierId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            HandlerId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 30
        };

        var result = await new FilterTripsDtoValidator().ValidateAsync(filter);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("page")]
    [InlineData("pageSize")]
    [InlineData("id")]
    [InlineData("type")]
    [InlineData("statusCount")]
    [InlineData("statusInvalid")]
    [InlineData("statusDuplicate")]
    [InlineData("createdFuture")]
    [InlineData("fromFuture")]
    [InlineData("toFuture")]
    [InlineData("range")]
    [InlineData("carrier")]
    [InlineData("warehouse")]
    [InlineData("handler")]
    public async Task FilterTripsDtoValidator_Should_RejectInvalidConstraintsAsync(string scenario)
    {
        var filter = new FilterTripsDto();
        switch (scenario)
        {
            case "page": filter.PageNumber = 0; break;
            case "pageSize": filter.PageSize = 31; break;
            case "id": filter.Id = Guid.Empty; break;
            case "type": filter.TripType = (TripType)999; break;
            case "statusCount": filter.Status = Enumerable.Repeat(TripStatus.Planned, 6).ToArray(); break;
            case "statusInvalid": filter.Status = [(TripStatus)999]; break;
            case "statusDuplicate": filter.Status = [TripStatus.Planned, TripStatus.Planned]; break;
            case "createdFuture": filter.CreatedAt = DateTime.UtcNow.AddDays(1); break;
            case "fromFuture": filter.From = DateTime.UtcNow.AddDays(1); break;
            case "toFuture": filter.To = DateTime.UtcNow.AddDays(1); break;
            case "range": filter.From = DateTime.UtcNow.AddDays(-1); filter.To = DateTime.UtcNow.AddDays(-2); break;
            case "carrier": filter.CarrierId = Guid.Empty; break;
            case "warehouse": filter.WarehouseId = Guid.Empty; break;
            case "handler": filter.HandlerId = Guid.Empty; break;
        }

        var result = await new FilterTripsDtoValidator().ValidateAsync(filter);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateVehicleDtoValidator_Should_AcceptValidDtoAsync()
    {
        var result = await new UpdateVehicleDtoValidator().ValidateAsync(ValidVehicle());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("type")]
    [InlineData("plate")]
    [InlineData("longPlate")]
    [InlineData("weight")]
    [InlineData("volume")]
    [InlineData("carrier")]
    public async Task UpdateVehicleDtoValidator_Should_RejectInvalidDtoAsync(string scenario)
    {
        var dto = ValidVehicle();
        switch (scenario)
        {
            case "type": dto.VehicleType = (VehicleType)999; break;
            case "plate": dto.PlateNumber = string.Empty; break;
            case "longPlate": dto.PlateNumber = new string('A', 51); break;
            case "weight": dto.CapacityWeight = 0; break;
            case "volume": dto.CapacityVolume = 0; break;
            case "carrier": dto.CarrierId = Guid.Empty; break;
        }

        var result = await new UpdateVehicleDtoValidator().ValidateAsync(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateVehicleCommandValidator_Should_AcceptUniqueVehicleAsync()
    {
        var rules = new Mock<IVehicleRulesRepository>();
        rules.Setup(x => x.PlateNumberExistsForAnotherVehicleAsync(It.IsAny<Guid>(), "ABC-123", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        rules.Setup(x => x.CarrierHasAnotherVehicleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var validator = new UpdateVehicleCommandValidator(new UpdateVehicleDtoValidator(), rules.Object);

        var result = await validator.ValidateAsync(new UpdateVehicleCommand(Guid.NewGuid(), Guid.NewGuid(), ValidVehicle()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateVehicleCommandValidator_Should_RejectDuplicatePlateAndCarrierVehicleAsync()
    {
        var rules = new Mock<IVehicleRulesRepository>();
        rules.Setup(x => x.PlateNumberExistsForAnotherVehicleAsync(It.IsAny<Guid>(), "ABC-123", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        rules.Setup(x => x.CarrierHasAnotherVehicleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var validator = new UpdateVehicleCommandValidator(new UpdateVehicleDtoValidator(), rules.Object);

        var result = await validator.ValidateAsync(new UpdateVehicleCommand(Guid.NewGuid(), Guid.NewGuid(), ValidVehicle()));

        result.Errors.Should().Contain(error => error.ErrorMessage == "Vehicle plate number already exists.");
        result.Errors.Should().Contain(error => error.ErrorMessage == "Carrier already has another vehicle.");
    }

    private static UpdateVehicleDto ValidVehicle() => new()
    {
        VehicleType = VehicleType.Van,
        PlateNumber = " ABC-123 ",
        CapacityWeight = 1000,
        CapacityVolume = 40,
        CarrierId = Guid.NewGuid()
    };
}

