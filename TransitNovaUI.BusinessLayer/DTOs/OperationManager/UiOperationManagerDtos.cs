using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;

namespace TransitNovaUI.BusinessLayer.DTOs.OperationManager;

public sealed class UiOperationManagerStatusStatDto
{
    public ShipmentStatuses Status { get; set; }
    public int Count { get; set; }

    public static UiOperationManagerStatusStatDto ToUiDto(OperationManagerStatusStatDto source) =>
        new() { Status = source.Status, Count = source.Count };
}

public sealed class UiOperationManagerActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public ShipmentStatuses Status { get; set; }

    public static UiOperationManagerActivityDto ToUiDto(OperationManagerActivityDto source) =>
        new()
        {
            Title = source.Title,
            Description = source.Description,
            OccurredAt = source.OccurredAt,
            Status = source.Status
        };
}

public sealed class UiRetrieveCarriersForOperationManagerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public CarrierStatus Status { get; set; }
    public int AssignedShipmentsCount { get; set; }
    public int ActiveTripsCount { get; set; }
    public List<string> ServedCities { get; set; } = [];
    public decimal Rating { get; set; }

    public static UiRetrieveCarriersForOperationManagerDto ToUiDto(
        RetrieveCarriersForOperationManagerDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            PhoneNumber = source.PhoneNumber,
            Status = source.Status,
            AssignedShipmentsCount = source.AssignedShipmentsCount,
            ActiveTripsCount = source.ActiveTripsCount,
            ServedCities = [.. source.ServedCities],
            Rating = source.Rating
        };
}

public sealed class UiOperationManagerDashboardDto
{
    public int TotalShipments { get; set; }
    public int PendingShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int ActiveShipments { get; set; }
    public IReadOnlyCollection<UiOperationManagerStatusStatDto> ShipmentStatistics { get; set; } = [];
    public IReadOnlyCollection<UiOperationManagerActivityDto> RecentActivity { get; set; } = [];
    public IReadOnlyCollection<UiRetrieveShipmentDto> RecentShipments { get; set; } = [];
    public IReadOnlyCollection<UiRetrieveCarriersForOperationManagerDto> RecentCarriers { get; set; } = [];

    public static UiOperationManagerDashboardDto ToUiDto(OperationManagerDashboardDto source) =>
        new()
        {
            TotalShipments = source.TotalShipments,
            PendingShipments = source.PendingShipments,
            DeliveredShipments = source.DeliveredShipments,
            ActiveShipments = source.ActiveShipments,
            ShipmentStatistics = source.ShipmentStatistics.Select(UiOperationManagerStatusStatDto.ToUiDto).ToList(),
            RecentActivity = source.RecentActivity.Select(UiOperationManagerActivityDto.ToUiDto).ToList(),
            RecentShipments = source.RecentShipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList(),
            RecentCarriers = source.RecentCarriers.Select(UiRetrieveCarriersForOperationManagerDto.ToUiDto).ToList()
        };
}

public sealed class UiProfileDashboardDto
{
    public int TotalShipments { get; set; }
    public int PendingShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int ActiveShipments { get; set; }
    public int IssueShipments { get; set; }
    public IReadOnlyCollection<UiRetrieveShipmentSummaryDto> ShipmentSummary { get; set; } = [];

    public static UiProfileDashboardDto ToUiDto(ProfileDashboardDto source) =>
        new()
        {
            TotalShipments = source.TotalShipments,
            PendingShipments = source.PendingShipments,
            DeliveredShipments = source.DeliveredShipments,
            ActiveShipments = source.ActiveShipments,
            IssueShipments = source.IssueShipments,
            ShipmentSummary = source.ShipmentSummary.Select(UiRetrieveShipmentSummaryDto.ToUiDto).ToList()
        };
}
