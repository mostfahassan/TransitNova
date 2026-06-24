using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{

    public class OperationManagerActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public ShipmentStatuses Status { get; set; }
    }

}
