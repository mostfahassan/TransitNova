using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.Carriers.Queries
{
    public sealed record GetCarrierShipmentsQuery(Guid CarrierId, CarrierShipmentFilterDto Filter)
        : IQuery<Result<CarrierShipmentListDto>>, ICachable
    {
        public string CacheKey => CacheKeys.CarrierShipments(CarrierId,Filter);
    }
}
