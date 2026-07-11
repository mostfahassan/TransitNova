using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Payment;
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
    UiAddressDto DeliveryAddress,
    UiAddressDto PickupAddress,
    Guid? PackageBundleId,
    Guid PaymentId,
    PaymentMethod PaymentMethod
    )
{
    public static CreateShipmentDto ToDto(UiCreateShipmentDto source) =>
        new(
            UiCreateReceiverDto.ToDto(source.Receiver),
            UiPackageSpecificationRequestDto.ToDto(source.PackageSpecification),
            source.Currency,
            source.PickUpDate,
            source.TransportationMode,
            source.ShipmentDeliveryType,
            UiAddressDto.ToDto(source.DeliveryAddress),
            UiAddressDto.ToDto(source.PickupAddress),
            source.PaymentId,
            source.PaymentMethod);
}
