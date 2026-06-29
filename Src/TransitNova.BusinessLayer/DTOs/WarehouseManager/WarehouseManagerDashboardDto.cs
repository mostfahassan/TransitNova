using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.DTOs.WarehouseManager
{
    public sealed class WarehouseManagerDashboardDto
    {
        public WarehouseManagerSummary Manager { get; set; } = new();
        public WarehouseManagerKpiDto Kpis { get; set; } = new();
        public List<RecentShipmentSummary> RecentShipments { get; set; } = [];
        public List<RecentTripSummary> RecentTrips { get; set; } = [];
    }

    public sealed class RecentShipmentSummary
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public ShipmentStatuses CurrentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class RecentTripSummary
    {
        public Guid Id { get; set; }
        public TripType TripType { get; set; }
        public TripStatus Status { get; set; }
        public DateTime PlannedDate { get; set; }
    }
    public sealed class WarehouseManagerSummary
    {
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
    }
    public sealed class WarehouseManagerKpiDto
    {
        public int TotalShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int InTransitShipments { get; set; }

        public int TotalTrips { get; set; }
        public int TotalCarriers { get; set; }
        public int ActiveCarriers { get; set; }
        public int ActiveTrips { get; set; }
        public int CompletedTrips { get; set; }
    }
}
