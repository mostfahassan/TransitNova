using FluentAssertions;
using TransitNova.BusinessLayer.Services.ShipmentServices;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.ApplicationLayer.Tests.Services;

public sealed class ShipmentPricingServicesTests
{
    private readonly ShipmentPricingServices _service = new();

    [Theory]
    [InlineData(enShipmentType.Standard, TransportationMode.Air, 250)]
    [InlineData(enShipmentType.Express, TransportationMode.Land, 450)]
    [InlineData(enShipmentType.Fragile, TransportationMode.Sea, 450)]
    public void CalculateShipment_Should_ApplyShipmentAndTransportMultipliers_When_ActualWeightIsChargeable(
        enShipmentType shipmentType,
        TransportationMode mode,
        decimal expectedCost)
    {
        var package = new PackageSpecification(10, 10, 10, 10);

        var (cost, _) = _service.CalculateShipment(package, shipmentType, mode);

        cost.Should().Be(expectedCost);
    }

    [Fact]
    public void CalculateShipment_Should_UseVolumetricWeight_When_ItExceedsActualWeight()
    {
        var package = new PackageSpecification(5, 100, 50, 40);

        var (cost, _) = _service.CalculateShipment(package, enShipmentType.Standard, TransportationMode.Air);

        cost.Should().Be(1000m);
    }

    [Fact]
    public void CalculateShipment_Should_ApplyMinimumCharge_When_CalculatedCostIsLower()
    {
        var package = new PackageSpecification(0.1m, 1, 1, 1);

        var (cost, _) = _service.CalculateShipment(package, enShipmentType.Standard, TransportationMode.Air);

        cost.Should().Be(50m);
    }

    [Theory]
    [InlineData(enShipmentType.Express, TransportationMode.Air, 1)]
    [InlineData(enShipmentType.Standard, TransportationMode.Land, 5)]
    [InlineData(enShipmentType.Fragile, TransportationMode.Sea, 10)]
    public void CalculateShipment_Should_ReturnDeliveryDateAtOrAfterBusinessEstimate_When_ModeAndTypeAreKnown(
        enShipmentType shipmentType,
        TransportationMode mode,
        int minimumDays)
    {
        var before = DateTime.UtcNow;

        var (_, deliveryDate) = _service.CalculateShipment(
            new PackageSpecification(1, 1, 1, 1),
            shipmentType,
            mode);

        deliveryDate.Should().BeOnOrAfter(before.AddDays(minimumDays));
        deliveryDate.Should().BeOnOrBefore(DateTime.UtcNow.AddDays(minimumDays + 2));
    }
}
