using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Carriers.Queries.Trips
{
    public sealed record GetCarrierTripDetailsQuery(Guid CarrierId, Guid TripId)
        : IQuery<Result<CarrierTripDto>>, ICachable
    {
        public string CacheKey => CacheKeys.Carriers.TripDetails(CarrierId,TripId);
    }
}

