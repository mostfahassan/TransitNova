using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed record UiRejectShipmentReason(string RejectionReason)
{
    public static RejectShipmentReason ToDto(UiRejectShipmentReason source) =>
        new(source.RejectionReason);

}
