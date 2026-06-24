using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.OperationManager;

public sealed class UiOperationManagerActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public ShipmentStatuses Status { get; set; }

    public static UiOperationManagerActivityDto ToUiDto(OperationManagerActivityDto source) =>
        new()
        {
            Title = source.Title,
            Description = source.Description,
            OccurredAt = source.OccurredAt,
            Status = source.Status
        };
}
