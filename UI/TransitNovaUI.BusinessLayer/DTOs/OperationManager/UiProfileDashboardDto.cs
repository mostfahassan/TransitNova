using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.OperationManager;

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
