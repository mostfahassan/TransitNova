using TransitNova.BusinessLayer.DTOs.WarehouseManager;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNovaUI.BusinessLayer.DTOs.WarehouseManager;

public sealed class UiWarehouseManagerDashboardDto
{
    public UiWarehouseManagerSummaryDto Manager { get; set; } = new();
    public UiWarehouseManagerKpiDto Kpis { get; set; } = new();
    public List<UiWarehouseManagerRecentShipmentSummaryDto> RecentShipments { get; set; } = [];
    public List<UiWarehouseManagerRecentTripSummaryDto> RecentTrips { get; set; } = [];

    public static UiWarehouseManagerDashboardDto ToUiDto(WarehouseManagerDashboardDto source) =>
        new()
        {
            Manager = UiWarehouseManagerSummaryDto.ToUiDto(source.Manager),
            Kpis = UiWarehouseManagerKpiDto.ToUiDto(source.Kpis),
            RecentShipments = source.RecentShipments.Select(UiWarehouseManagerRecentShipmentSummaryDto.ToUiDto).ToList(),
            RecentTrips = source.RecentTrips.Select(UiWarehouseManagerRecentTripSummaryDto.ToUiDto).ToList()
        };
}

public sealed class UiWarehouseManagerSummaryDto
{
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public static UiWarehouseManagerSummaryDto ToUiDto(WarehouseManagerSummary source) =>
        new()
        {
            ManagerId = source.ManagerId,
            ManagerName = source.ManagerName,
            WarehouseId = source.WarehouseId,
            WarehouseName = source.WarehouseName
        };
}

public sealed class UiWarehouseManagerKpiDto
{
    public int TotalShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int InTransitShipments { get; set; }
    public int TotalTrips { get; set; }
    public int TotalCarriers { get; set; }
    public int ActiveCarriers { get; set; }
    public int ActiveTrips { get; set; }
    public int CompletedTrips { get; set; }

    public static UiWarehouseManagerKpiDto ToUiDto(WarehouseManagerKpiDto source) =>
        new()
        {
            TotalShipments = source.TotalShipments,
            DeliveredShipments = source.DeliveredShipments,
            InTransitShipments = source.InTransitShipments,
            TotalTrips = source.TotalTrips,
            TotalCarriers = source.TotalCarriers,
            ActiveCarriers = source.ActiveCarriers,
            ActiveTrips = source.ActiveTrips,
            CompletedTrips = source.CompletedTrips
        };
}

public sealed class UiWarehouseManagerRecentShipmentSummaryDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public ShipmentStatuses CurrentStatus { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiWarehouseManagerRecentShipmentSummaryDto ToUiDto(RecentShipmentSummary source) =>
        new()
        {
            Id = source.Id,
            TrackingNumber = source.TrackingNumber,
            CurrentStatus = source.CurrentStatus,
            CreatedAt = source.CreatedAt
        };
}

public sealed class UiWarehouseManagerRecentTripSummaryDto
{
    public Guid Id { get; set; }
    public TripType TripType { get; set; }
    public TripStatus Status { get; set; }
    public DateTime PlannedDate { get; set; }

    public static UiWarehouseManagerRecentTripSummaryDto ToUiDto(RecentTripSummary source) =>
        new()
        {
            Id = source.Id,
            TripType = source.TripType,
            Status = source.Status,
            PlannedDate = source.PlannedDate
        };
}
