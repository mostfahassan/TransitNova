using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{
    public class OperationManagerDashboardDto
    {
        public int TotalShipments { get; set; }
        public int PendingShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int ActiveShipments { get; set; }
        public IReadOnlyCollection<OperationManagerStatusStatDto> ShipmentStatistics { get; set; } = [];
        public IReadOnlyCollection<OperationManagerActivityDto> RecentActivity { get; set; } = [];
        public IReadOnlyCollection<RetrieveShipmentDto> RecentShipments { get; set; } = [];
        public IReadOnlyCollection <RetrieveCarriersForOperationManagerDto> RecentCarriers { get; set; } = [];
    }

}
