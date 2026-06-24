using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{

    public class OperationManagerShipmentStatusUpdateDto
    {
        public ShipmentStatuses Status { get; set; }
        public string? RejectionReason { get; set; }
    }

}
