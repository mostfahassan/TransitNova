using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public record UpdateShipmentDto
     (
      Guid? ReceiverId,
      AddressDto? DeliveryAddress,
      AddressDto? PickupAddress,
      PackageSpecificationDto? PackageSpecification,
      enShipmentType? ShipmentType,
      TransportationMode? TransportationMode
     );
}
