using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.DTOs.ShipmentStatusDto
{
    public class RetrieveShipmentStatusDto
    {
        public ShipmentStatuses StatusType { get; set; }
        public DateTime ChangedAt { get; set; }
        public UserSummaryDto? Carrier { get; set; }
    }
}
