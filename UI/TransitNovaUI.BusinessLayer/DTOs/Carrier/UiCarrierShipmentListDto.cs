using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiCarrierShipmentListDto
{
    public UiPagedResult<UiRetrieveShipmentDto> Shipments { get; set; } =
        UiPagedResult<UiRetrieveShipmentDto>.From([], 0, 1, 12);

    public IReadOnlyCollection<UiCarrierStatusStatDto> Statistics { get; set; } = [];

    public static UiCarrierShipmentListDto ToUiDto(CarrierShipmentListDto source) =>
        new()
        {
            Shipments = UiPagedResult<UiRetrieveShipmentDto>.From(
                source.Shipments.Data.Select(UiRetrieveShipmentDto.ToUiDto),
                source.Shipments.TotalCount,
                source.Shipments.PageNumber,
                source.Shipments.PageSize),
            Statistics = source.Statistics.Select(UiCarrierStatusStatDto.ToUiDto).ToList()
        };
}
