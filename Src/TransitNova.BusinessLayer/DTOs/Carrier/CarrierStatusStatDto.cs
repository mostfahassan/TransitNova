using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{

    public class CarrierStatusStatDto
    {
        public ShipmentStatuses Status { get; set; }
        public int Count { get; set; }
    }

}
