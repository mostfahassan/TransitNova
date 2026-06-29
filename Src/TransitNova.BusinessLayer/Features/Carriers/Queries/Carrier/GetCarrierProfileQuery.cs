using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier
{
    public sealed record GetCarrierProfileQuery(Guid CarrierId)
        : IQuery<Result<CarrierProfileDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.Profile(CarrierId);
    }
}

