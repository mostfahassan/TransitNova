using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier
{
    public record GetCarrierRatingQuery(Guid CarrierId) : IQuery<Result<decimal>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.Rating(CarrierId);
    }
}