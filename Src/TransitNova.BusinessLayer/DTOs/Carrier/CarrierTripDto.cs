using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Entities.Common;
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
        public Address WarehouseAddress { get; set; } = null!;
        public int ShipmentCount { get; set; }
        public IReadOnlyCollection<RetrieveShipmentDto> Shipments { get; set; } = [];
    }

}
