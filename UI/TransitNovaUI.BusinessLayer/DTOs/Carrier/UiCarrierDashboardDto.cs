using TransitNova.BusinessLayer.DTOs.Carrier;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierDashboardDto
{
    public Guid CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;
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
            CarrierId = source.Id,
            CarrierName = source.CarrierName ?? string.Empty,
            AssignedShipmentsCount = source.AssignedShipmentsCount,
            DeliveredShipmentsCount = source.DeliveredShipmentsCount,
            PendingShipmentsCount = source.PendingShipmentsCount,
            ActiveTripsCount = source.ActiveTripsCount,
            RevenueSummary = source.RevenueSummary,
            ShipmentStatistics = [..source.ShipmentStatistics.Select(UiCarrierStatusStatDto.ToUiDto).ToList()],
            RecentShipments = [..source.RecentShipments.Select(UiRetrieveShipmentDto.ToUiDto).ToList()],
            ActiveTrips = [..source.ActiveTrips.Select(UiCarrierTripDto.ToUiDto).ToList()],
            RecentActivity = [..source.RecentActivity.Select(UiCarrierActivityDto.ToUiDto).ToList()]
        };
}
