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
