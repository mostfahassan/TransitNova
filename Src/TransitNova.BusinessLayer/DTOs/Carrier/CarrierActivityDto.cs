using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class CarrierActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ShipmentStatuses? Status { get; set; }
        public DateTime OccurredAt { get; set; }
    }

}
