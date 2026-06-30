using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment
{
    public sealed record GetCarrierShipmentsQuery(Guid CarrierId, CarrierShipmentFilterDto Filter)
        : IQuery<Result<CarrierShipmentListDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.Shipments(CarrierId,Filter);
    }
}

