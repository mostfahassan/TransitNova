using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.OperationManager;

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
