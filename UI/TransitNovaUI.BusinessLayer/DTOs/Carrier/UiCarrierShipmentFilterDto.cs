using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierShipmentFilterDto
{
    public ShipmentStatuses? Status { get; set; }
    public TransportationMode? Mode { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public static CarrierShipmentFilterDto ToDto(UiCarrierShipmentFilterDto source) =>
        new()
        {
            Status = source.Status,
            Mode = source.Mode,
            SearchTerm = source.SearchTerm,
            SortBy = source.SortBy,
            SortDescending = source.SortDescending,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };

}
