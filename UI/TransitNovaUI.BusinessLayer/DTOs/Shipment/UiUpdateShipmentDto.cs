using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed record UiUpdateShipmentDto(
    Guid? ReceiverId,
    string? DeliveryAddress,
    string? PickupAddress,
    UiPackageSpecificationRequestDto? PackageSpecification,
    enShipmentType? ShipmentType,
    TransportationMode? TransportationMode)
{
    public static UpdateShipmentDto ToDto(UiUpdateShipmentDto source) =>
        new(
            source.ReceiverId,
            source.DeliveryAddress,
            source.PickupAddress,
            source.PackageSpecification is null
                ? null
                : UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.ShipmentType,
            source.TransportationMode);

}
