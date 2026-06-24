using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{

    public class CarrierTripDto
    {
        public Guid Id { get; set; }
        public TripType TripType { get; set; }
        public TripStatus Status { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseAddress { get; set; } = string.Empty;
        public int ShipmentCount { get; set; }
        public IReadOnlyCollection<RetrieveShipmentDto> Shipments { get; set; } = [];
    }

}
