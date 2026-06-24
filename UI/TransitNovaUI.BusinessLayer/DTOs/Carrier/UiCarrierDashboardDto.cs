using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

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
