using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Location.Countries.Queries
{
    public sealed record GetCountryGovernmentsQuery(int countryId) : IQuery<Result<IEnumerable<GovernmentDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Countries.Governments(countryId);
    };
   
}
