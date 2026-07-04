using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{
    public class OperationManagerDashboardDto
    {
        public int TotalShipments { get; set; }
        public int PendingShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int ActiveShipments { get; set; }
        public int TotalHandedShipments { get; set; }
        public int TotalHandedCarriers { get; set; }
        public int IssueShipments { get; set; }
        public int CancelledShipments { get; set; }
        public int TotalPickupShipments { get; set; }
        public IReadOnlyCollection<OperationManagerStatusStatDto> ShipmentStatistics { get; set; } = [];
        public IReadOnlyCollection<OperationManagerActivityDto> RecentActivity { get; set; } = [];
        public IReadOnlyCollection<RetrieveShipmentSummaryDto> RecentShipments { get; set; } = [];
        public IReadOnlyCollection <RetrieveCarriersForOperationManagerDto> RecentCarriers { get; set; } = [];
    }

}
