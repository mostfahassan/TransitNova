using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Shipments.Queries
{
    public record FilterShipmentsQuery(ShipmentFilterDto FilterCriteria)
        : IQuery<Result<PagedResult<RetrieveShipmentDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Shipments.Filter(FilterCriteria);
    }
}
