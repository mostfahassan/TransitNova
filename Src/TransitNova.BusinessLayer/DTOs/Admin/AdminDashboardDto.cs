
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{
    public sealed class AdminDashboardDto
    {
        public AdminKpiDto Kpis { get; set; } = new();

        public List<ShipmentStatusStatDto> ShipmentStatistics { get; set; } = [];

        public RevenueSummaryDto RevenueSummary { get; set; } = new();

        public List<RetrieveShipmentDto> RecentShipments { get; set; } = [];

        public List<AdminActivityDto> RecentActivities { get; set; } = [];

        public AdminOperationalHealthDto OperationalHealth { get; set; } = new();

        public List<TopCarrierDto> TopCarriers { get; set; } = [];
        public List<TopOperationManagerDto> TopOperationManagers { get; set; } = [];
    }

}
