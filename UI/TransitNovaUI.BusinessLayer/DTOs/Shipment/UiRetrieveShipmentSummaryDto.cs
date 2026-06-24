using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed class UiRetrieveShipmentSummaryDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string ReceiverCity { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public decimal ShippinCost { get; set; }
    public decimal Weight { get; set; }
    public ShipmentStatuses CurrentStatus { get; set; }
    public enShipmentType ShipmentType { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiRetrieveShipmentSummaryDto ToUiDto(RetrieveShipmentSummaryDto source) =>
        new()
        {
            Id = source.Id,
            TrackingNumber = source.TrackingNumber,
            ReceiverCity = source.ReceiverCity,
            SenderCity = source.SenderCity,
            ShippinCost = source.ShippinCost,
            Weight = source.Weight,
            CurrentStatus = source.CurrentStatus,
            ShipmentType = source.ShipmentType,
            EstimatedDeliveryDate = source.EstimatedDeliveryDate,
            CreatedAt = source.CreatedAt
        };

    public static UiPagedResult<UiRetrieveShipmentSummaryDto> ToUiPagedDto(
        PagedResult<RetrieveShipmentSummaryDto> source) =>
        UiPagedResult<UiRetrieveShipmentSummaryDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}
