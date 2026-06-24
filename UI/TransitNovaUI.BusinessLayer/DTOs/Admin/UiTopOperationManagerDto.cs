using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiTopOperationManagerDto
{
    public Guid OperationManagerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ApprovedShipments { get; set; }

    public static UiTopOperationManagerDto ToUiDto(TopOperationManagerDto source) =>
        new()
        {
            OperationManagerId = source.OperationManagerId,
            FullName = source.FullName,
            ApprovedShipments = source.ApprovedShipments
        };
}
