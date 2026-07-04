using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{
    public class OperationManagerStatusStatDto
    {
        public ShipmentStatuses Status { get; set; }
        public int Count { get; set; }
    }
}
