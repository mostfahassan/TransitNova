
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public record CreateShipmentDto
    (
        CreateReceiverDto Receiver,
        PackageSpecificationDto PackageSpecification,
        Currency Currency,
        DateTime? PickUpDate,
        TransportationMode TransportationMode,
        enShipmentType ShipmentDeliveryType,
        string DeliveryAddress,
        string PickupAddress,
        Guid? PackageBundleId,
        Guid PaymentId ,
        PaymentMethod PaymentMethod

    );
}
