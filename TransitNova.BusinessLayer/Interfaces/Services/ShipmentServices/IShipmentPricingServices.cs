using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices
{
    public interface IShipmentPricingServices
    { 
       public (decimal,DateTime) CalculateShipment(PackageSpecification packageSpecification, enShipmentType shipmentType, TransportationMode mode);       
    }
}
