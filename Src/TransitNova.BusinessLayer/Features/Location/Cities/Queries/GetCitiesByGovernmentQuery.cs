using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Queries
{
    public sealed record GetCitiesByGovernmentQuery(int GovernmentId) : IQuery<Result<IEnumerable<CityDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.CitiesByGovernment(GovernmentId);
    }
}
