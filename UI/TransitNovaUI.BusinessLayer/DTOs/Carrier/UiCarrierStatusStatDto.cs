using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierStatusStatDto
{
    public ShipmentStatuses Status { get; set; }
    public int Count { get; set; }

    public static UiCarrierStatusStatDto ToUiDto(CarrierStatusStatDto source) =>
        new() { Status = source.Status, Count = source.Count };
}
