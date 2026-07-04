
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.Shipments.Queries
{
    public sealed record GetShipmentStatisticsQuery() : IQuery<Result<Dictionary<ShipmentStatuses, int>>>, ICachable
    {
        public string CacheKey => CacheKeys.Shipments.Statistics;
    }
}
