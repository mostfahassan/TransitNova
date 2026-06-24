using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

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
