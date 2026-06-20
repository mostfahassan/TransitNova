using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public class RetrieveShipmentSummaryDto
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string ReceiverCity { get; set; } = string.Empty;
        public string SenderCity { get; set; } = string.Empty;
        public decimal ShippinCost { get; set; }
        public decimal Weight { get; set; } = new();
        public ShipmentStatuses CurrentStatus { get; set; }
        public enShipmentType ShipmentType { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
