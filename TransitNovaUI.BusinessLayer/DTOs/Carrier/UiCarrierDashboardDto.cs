using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;

namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierStatusStatDto
{
    public ShipmentStatuses Status { get; set; }
    public int Count { get; set; }

    public static UiCarrierStatusStatDto ToUiDto(CarrierStatusStatDto source) =>
        new() { Status = source.Status, Count = source.Count };
}

public sealed class UiCarrierActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ShipmentStatuses? Status { get; set; }
    public DateTime OccurredAt { get; set; }

    public static UiCarrierActivityDto ToUiDto(CarrierActivityDto source) =>
        new()
        {
            Title = source.Title,
            Description = source.Description,
            Status = source.Status,
            OccurredAt = source.OccurredAt
        };
}

public sealed class UiCarrierTripDto
{
    public Guid Id { get; set; }
    public TripType TripType { get; set; }
    public TripStatus Status { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string WarehouseAddress { get; set; } = string.Empty;
    public int ShipmentCount { get; set; }
    public IReadOnlyCollection<UiRetrieveShipmentDto> Shipments { get; set; } = [];

    public static UiCarrierTripDto ToUiDto(CarrierTripDto source) =>
        new()
        {
            Id = source.Id,
            TripType = source.TripType,
            Status = source.Status,
            PlannedDate = source.PlannedDate,
            StartTime = source.StartTime,
            EndTime = source.EndTime,
            WarehouseName = source.WarehouseName,
            WarehouseAddress = source.WarehouseAddress,
            ShipmentCount = source.ShipmentCount,
            Shipments = source.Shipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList()
        };
}

public sealed class UiCarrierShipmentFilterDto
{
    public ShipmentStatuses? Status { get; set; }
    public TransportationMode? Mode { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public static CarrierShipmentFilterDto ToDto(UiCarrierShipmentFilterDto source) =>
        new()
        {
            Status = source.Status,
            Mode = source.Mode,
            SearchTerm = source.SearchTerm,
            SortBy = source.SortBy,
            SortDescending = source.SortDescending,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };

}

public sealed class UiCarrierShipmentListDto
{
    public UiPagedResult<UiRetrieveShipmentDto> Shipments { get; set; } =
        UiPagedResult<UiRetrieveShipmentDto>.From([], 0, 1, 12);

    public IReadOnlyCollection<UiCarrierStatusStatDto> Statistics { get; set; } = [];

    public static UiCarrierShipmentListDto ToUiDto(CarrierShipmentListDto source) =>
        new()
        {
            Shipments = UiPagedResult<UiRetrieveShipmentDto>.From(
                source.Shipments.Data.Select(UiRetrieveShipmentDto.ToUiDto),
                source.Shipments.TotalCount,
                source.Shipments.PageNumber,
                source.Shipments.PageSize),
            Statistics = source.Statistics.Select(UiCarrierStatusStatDto.ToUiDto).ToList()
        };
}

public sealed class UiCarrierDashboardDto
{
    public UiCarrierProfileDto Profile { get; set; } = new();
    public int AssignedShipmentsCount { get; set; }
    public int DeliveredShipmentsCount { get; set; }
    public int PendingShipmentsCount { get; set; }
    public int ActiveTripsCount { get; set; }
    public decimal RevenueSummary { get; set; }
    public IReadOnlyCollection<UiCarrierStatusStatDto> ShipmentStatistics { get; set; } = [];
    public IReadOnlyCollection<UiRetrieveShipmentDto> RecentShipments { get; set; } = [];
    public IReadOnlyCollection<UiCarrierTripDto> ActiveTrips { get; set; } = [];
    public IReadOnlyCollection<UiCarrierActivityDto> RecentActivity { get; set; } = [];

    public static UiCarrierDashboardDto ToUiDto(CarrierDashboardDto source) =>
        new()
        {
            Profile = UiCarrierProfileDto.ToUiDto(source.Profile),
            AssignedShipmentsCount = source.AssignedShipmentsCount,
            DeliveredShipmentsCount = source.DeliveredShipmentsCount,
            PendingShipmentsCount = source.PendingShipmentsCount,
            ActiveTripsCount = source.ActiveTripsCount,
            RevenueSummary = source.RevenueSummary,
            ShipmentStatistics = source.ShipmentStatistics.Select(UiCarrierStatusStatDto.ToUiDto).ToList(),
            RecentShipments = source.RecentShipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList(),
            ActiveTrips = source.ActiveTrips.Select(UiCarrierTripDto.ToUiDto).ToList(),
            RecentActivity = source.RecentActivity.Select(UiCarrierActivityDto.ToUiDto).ToList()
        };
}
