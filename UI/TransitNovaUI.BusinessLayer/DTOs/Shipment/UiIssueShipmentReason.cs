using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed record UiIssueShipmentReason(string IssueReason)
{
    public static IssueShipmentReason ToDto(UiIssueShipmentReason source) =>
        new(source.IssueReason);

}
