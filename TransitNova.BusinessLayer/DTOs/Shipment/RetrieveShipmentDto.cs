

using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Shipment
{
    public class RetrieveShipmentDto
    {
        public Guid Id { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
        public UserSummaryDto Receiver { get; set; } = null!;
        public UserSummaryDto? Sender { get; set; } = null!;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public PackageSpecificationDto PackageSpecification { get; set; } = new();
        public Currency Currency { get; set; }
        public TransportationMode TransportationMode { get; set; }
        public ShipmentStatuses CurrentStatus { get; set; }
        public decimal ShippingCost { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public PaymentSummaryDto? Payment { get; set; }
        public List<RetrieveShipmentStatusDto> ShipmentStates { get; set; } = new();
        public enShipmentType ShipmentType { get; set; }
        public Guid? PackageBundleId { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
