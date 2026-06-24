using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.OperationManager;

public sealed class UiOperationManagerStatusStatDto
{
    public ShipmentStatuses Status { get; set; }
    public int Count { get; set; }

    public static UiOperationManagerStatusStatDto ToUiDto(OperationManagerStatusStatDto source) =>
        new() { Status = source.Status, Count = source.Count };
}
