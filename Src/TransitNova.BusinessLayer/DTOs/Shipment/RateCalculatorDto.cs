using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public sealed record RateCalculatorDto(PackageSpecificationDto PackageSpecification, TransportationMode TransportationMode, enShipmentType ShipmentDeliveryType);
}
