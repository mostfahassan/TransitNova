using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public record UpdateShipmentDto
     (
      Guid? ReceiverId,
      string? DeliveryAddress,
      string? PickupAddress,
      PackageSpecificationDto? PackageSpecification,
      enShipmentType? ShipmentType,
      TransportationMode? TransportationMode
     );
}
