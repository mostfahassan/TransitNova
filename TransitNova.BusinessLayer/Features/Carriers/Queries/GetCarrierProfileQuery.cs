using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries
{
    public sealed record GetCarrierProfileQuery(Guid CarrierId)
        : IQuery<Result<CarrierProfileDto>>, ICachable
    {
        public string CacheKey => CacheKeys.CarrierProfile(CarrierId);
    }
}
