using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public class ShipmentFilterDto
    {
        public ShipmentStatuses[]? Status { get; set; }
        public TransportationMode? Mode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? SenderId { get; set; }
        public Guid? WarehouseId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
