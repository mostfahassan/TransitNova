using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ShipmentStatuses? Status { get; set; }
    public DateTime OccurredAt { get; set; }

    public static UiCarrierActivityDto ToUiDto(CarrierActivityDto source) =>
        new()
        {
            Title = source.Title,
            Description = source.Description,
            Status = source.Status,
            OccurredAt = source.OccurredAt
        };
}
