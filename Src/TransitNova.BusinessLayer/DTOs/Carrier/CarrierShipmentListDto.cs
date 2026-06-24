using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{

    public class CarrierShipmentListDto
    {
        public PagedResult<RetrieveShipmentDto> Shipments { get; set; } =
            PagedResult<RetrieveShipmentDto>.From([], 0, 1, 12);
        public IReadOnlyCollection<CarrierStatusStatDto> Statistics { get; set; } = [];
    }

}
