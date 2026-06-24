using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.Common.CommonData;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed record UiCreateShipmentDto(
    UiCreateReceiverDto Receiver,
    UiPackageSpecificationRequestDto PackageSpecification,
    Currency Currency,
    DateTime? PickUpDate,
    TransportationMode TransportationMode,
    enShipmentType ShipmentDeliveryType,
    string DeliveryAddress,
    string PickupAddress,
    Guid? PackageBundleId)
{
    public static CreateShipmentDto ToDto(UiCreateShipmentDto source) =>
        new(
            UiCreateReceiverDto.ToDto(source.Receiver),
            UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.Currency,
            source.PickUpDate,
            source.TransportationMode,
            source.ShipmentDeliveryType,
            source.DeliveryAddress,
            source.PickupAddress,
            source.PackageBundleId);

}
