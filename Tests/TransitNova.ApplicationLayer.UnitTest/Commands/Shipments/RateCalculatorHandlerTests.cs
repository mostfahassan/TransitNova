using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Commands;
using TransitNova.BusinessLayer.Features.Shipments.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Commands.Shipments;

public sealed class RateCalculatorHandlerTests
{
    [Fact]
    public async Task RateCalculatorHandler_Should_MapDtoAndCallPricingServiceAsync()
    {
        var pricingService = new Mock<IShipmentPricingServices>();
        pricingService.Setup(x => x.CalculateShipment(
                It.IsAny<PackageSpecification>(),
                enShipmentType.Express,
                TransportationMode.Air))
            .Returns((250m, DateTime.UtcNow.AddDays(1)));
        var handler = new RateCalculatorHandler(pricingService.Object, NullLogger<RateCalculatorHandler>.Instance);
        var command = new RateCalculatorCommand(new RateCalculatorDto(
            new PackageSpecificationDto
            {
                Weight = 10,
                Width = 20,
                Height = 30,
                Length = 40
            },
            TransportationMode.Air,
            enShipmentType.Express));

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(250m);
        pricingService.Verify(x => x.CalculateShipment(
            It.Is<PackageSpecification>(p =>
                p.Weight == 10 &&
                p.Width == 20 &&
                p.Height == 30 &&
                p.Length == 40),
            enShipmentType.Express,
            TransportationMode.Air), Times.Once);
    }

    [Fact]
    public async Task RateCalculatorHandler_WhenPricingServiceThrows_ShouldPropagateAsync()
    {
        var pricingService = new Mock<IShipmentPricingServices>();
        pricingService.Setup(x => x.CalculateShipment(
                It.IsAny<PackageSpecification>(),
                It.IsAny<enShipmentType>(),
                It.IsAny<TransportationMode>()))
            .Throws(new InvalidOperationException("pricing unavailable"));
        var handler = new RateCalculatorHandler(pricingService.Object, NullLogger<RateCalculatorHandler>.Instance);
        var command = new RateCalculatorCommand(new RateCalculatorDto(
            new PackageSpecificationDto { Weight = 1, Width = 1, Height = 1, Length = 1 },
            TransportationMode.Land,
            enShipmentType.Standard));

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("pricing unavailable");
    }
}
