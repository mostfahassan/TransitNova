using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.Common.CommonData;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed record UiUpdateShipmentDto(
    Guid? ReceiverId,
    UiAddressDto? DeliveryAddress,
    UiAddressDto? PickupAddress,
    UiPackageSpecificationRequestDto? PackageSpecification,
    enShipmentType? ShipmentType,
    TransportationMode? TransportationMode)
{
    public static UpdateShipmentDto ToDto(UiUpdateShipmentDto source) =>
        new(
            source.ReceiverId,
            source.DeliveryAddress is null ? null : UiAddressDto.ToDto(source.DeliveryAddress),
            source.PickupAddress is null ? null : UiAddressDto.ToDto(source.PickupAddress),
            source.PackageSpecification is null
                ? null
                : UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.ShipmentType,
            source.TransportationMode);

}
