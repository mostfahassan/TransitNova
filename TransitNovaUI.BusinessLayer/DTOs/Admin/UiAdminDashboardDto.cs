using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;

namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiShipmentStatusStatDto
{
    public ShipmentStatuses Status { get; set; }
    public int Count { get; set; }

    public static UiShipmentStatusStatDto ToUiDto(ShipmentStatusStatDto source) =>
        new() { Status = source.Status, Count = source.Count };
}

public sealed class UiAdminKpiDto
{
    public int TotalShipments { get; set; }
    public int ActiveShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int PendingShipments { get; set; }
    public int TotalUsers { get; set; }
    public int TotalCarriers { get; set; }
    public int TotalOperationManagers { get; set; }
    public int ActiveTrips { get; set; }

    public static UiAdminKpiDto ToUiDto(AdminKpiDto source) =>
        new()
        {
            TotalShipments = source.TotalShipments,
            ActiveShipments = source.ActiveShipments,
            DeliveredShipments = source.DeliveredShipments,
            PendingShipments = source.PendingShipments,
            TotalUsers = source.TotalUsers,
            TotalCarriers = source.TotalCarriers,
            TotalOperationManagers = source.TotalOperationManagers,
            ActiveTrips = source.ActiveTrips
        };
}

public sealed class UiAdminOperationalHealthDto
{
    public int AvailableCarriers { get; set; }
    public int BusyCarriers { get; set; }
    public int ActiveOperationManagers { get; set; }
    public decimal AverageCarrierRating { get; set; }
    public decimal DeliverySuccessRate { get; set; }
    public decimal CancelledShipmentRate { get; set; }

    public static UiAdminOperationalHealthDto ToUiDto(AdminOperationalHealthDto source) =>
        new()
        {
            AvailableCarriers = source.AvailableCarriers,
            BusyCarriers = source.BusyCarriers,
            ActiveOperationManagers = source.ActiveOperationManagers,
            AverageCarrierRating = source.AverageCarrierRating,
            DeliverySuccessRate = source.DeliverySuccessRate,
            CancelledShipmentRate = source.CancelledShipmentRate
        };
}

public sealed class UiRevenueSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public decimal DailyRevenue { get; set; }

    public static UiRevenueSummaryDto ToUiDto(RevenueSummaryDto source) =>
        new()
        {
            TotalRevenue = source.TotalRevenue,
            MonthlyRevenue = source.MonthlyRevenue,
            WeeklyRevenue = source.WeeklyRevenue,
            DailyRevenue = source.DailyRevenue
        };
}

public sealed class UiAdminActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;

    public static UiAdminActivityDto ToUiDto(AdminActivityDto source) =>
        new()
        {
            Title = source.Title,
            Description = source.Description,
            OccurredAt = source.OccurredAt,
            PerformedBy = source.PerformedBy
        };
}

public sealed class UiTopCarrierDto
{
    public Guid CarrierId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int DeliveredShipments { get; set; }
    public decimal Rating { get; set; }

    public static UiTopCarrierDto ToUiDto(TopCarrierDto source) =>
        new()
        {
            CarrierId = source.CarrierId,
            FullName = source.FullName,
            DeliveredShipments = source.DeliveredShipments,
            Rating = source.Rating
        };
}

public sealed class UiTopOperationManagerDto
{
    public Guid OperationManagerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ApprovedShipments { get; set; }

    public static UiTopOperationManagerDto ToUiDto(TopOperationManagerDto source) =>
        new()
        {
            OperationManagerId = source.OperationManagerId,
            FullName = source.FullName,
            ApprovedShipments = source.ApprovedShipments
        };
}

public sealed class UiAdminDashboardDto
{
    public UiAdminKpiDto Kpis { get; set; } = new();
    public List<UiShipmentStatusStatDto> ShipmentStatistics { get; set; } = [];
    public UiRevenueSummaryDto RevenueSummary { get; set; } = new();
    public List<UiRetrieveShipmentDto> RecentShipments { get; set; } = [];
    public List<UiAdminActivityDto> RecentActivities { get; set; } = [];
    public UiAdminOperationalHealthDto OperationalHealth { get; set; } = new();
    public List<UiTopCarrierDto> TopCarriers { get; set; } = [];
    public List<UiTopOperationManagerDto> TopOperationManagers { get; set; } = [];

    public static UiAdminDashboardDto ToUiDto(AdminDashboardDto source) =>
        new()
        {
            Kpis = UiAdminKpiDto.ToUiDto(source.Kpis),
            ShipmentStatistics = source.ShipmentStatistics.Select(UiShipmentStatusStatDto.ToUiDto).ToList(),
            RevenueSummary = UiRevenueSummaryDto.ToUiDto(source.RevenueSummary),
            RecentShipments = source.RecentShipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList(),
            RecentActivities = source.RecentActivities.Select(UiAdminActivityDto.ToUiDto).ToList(),
            OperationalHealth = UiAdminOperationalHealthDto.ToUiDto(source.OperationalHealth),
            TopCarriers = source.TopCarriers.Select(UiTopCarrierDto.ToUiDto).ToList(),
            TopOperationManagers = source.TopOperationManagers.Select(UiTopOperationManagerDto.ToUiDto).ToList()
        };
}
