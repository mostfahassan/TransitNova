using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Queries
{
    public sealed record FilterCountriesQuery(CountryFilterDto Filter) : IQuery<Result<PagedResult<CountryDto>>>, ICachable
    {
        public string CacheKey => CacheKeys.Countries.Filter(Filter);
    }
}

