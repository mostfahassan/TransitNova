using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed class UiRateCalculatorDto
{
    public UiPackageSpecificationRequestDto PackageSpecification { get; set; } = new();
    public TransportationMode TransportationMode { get; set; }
    public enShipmentType ShipmentDeliveryType { get; set; }

    public static RateCalculatorDto ToDto(UiRateCalculatorDto source) =>
        new(
            UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.TransportationMode,
            source.ShipmentDeliveryType);
}
