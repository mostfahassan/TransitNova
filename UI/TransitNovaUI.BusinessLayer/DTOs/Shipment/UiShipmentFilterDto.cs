using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Shipment;

public sealed class UiShipmentFilterDto
{
    public ShipmentStatuses[]? Status { get; set; }
    public TransportationMode? Mode { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Guid? SenderId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public static ShipmentFilterDto ToDto(UiShipmentFilterDto source) =>
        new()
        {
            Status = source.Status is null ? null : [.. source.Status],
            Mode = source.Mode,
            From = source.From,
            To = source.To,
            SenderId = source.SenderId,
            SearchTerm = source.SearchTerm,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };

}
