using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{
    public class ProfileDashboardDto
    {
        public int TotalShipments { get; set; }
        public int PendingShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int ActiveShipments { get; set; }
        public int IssueShipments { get; set; }
        public IReadOnlyCollection<RetrieveShipmentSummaryDto> ShipmentSummary { get; set; } = [];
    }

}
