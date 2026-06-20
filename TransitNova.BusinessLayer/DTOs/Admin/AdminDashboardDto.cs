
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

    public class ShipmentStatusStatDto
    {
        public ShipmentStatuses Status { get; set; }
        public int Count { get; set; }
    }

    public sealed class AdminKpiDto
    {
        public int TotalShipments { get; set; }

        public int ActiveShipments { get; set; }

        public int DeliveredShipments { get; set; }

        public int PendingShipments { get; set; }

        public int TotalUsers { get; set; }

        public int TotalCarriers { get; set; }

        public int TotalOperationManagers { get; set; }

        public int ActiveTrips { get; set; }
    }


    public sealed class AdminOperationalHealthDto
    {
        public int AvailableCarriers { get; set; }

        public int BusyCarriers { get; set; }

        public int ActiveOperationManagers { get; set; }

        public decimal AverageCarrierRating { get; set; }

        public decimal DeliverySuccessRate { get; set; }

        public decimal CancelledShipmentRate { get; set; }
    }

    public sealed class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal MonthlyRevenue { get; set; }

        public decimal WeeklyRevenue { get; set; }

        public decimal DailyRevenue { get; set; }
    }

    public sealed class AdminActivityDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }

        public string PerformedBy { get; set; } = string.Empty;
    }

    public sealed class TopCarrierDto
    {
        public Guid CarrierId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int DeliveredShipments { get; set; }

        public decimal Rating { get; set; }
    }

    public sealed class TopOperationManagerDto
    {
        public Guid OperationManagerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int ApprovedShipments { get; set; }
    }

   
}
