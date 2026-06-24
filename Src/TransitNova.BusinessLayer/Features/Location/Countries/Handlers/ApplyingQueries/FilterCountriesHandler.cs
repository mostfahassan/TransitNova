using FluentValidation;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyingQueries
{
    public sealed class FilterCountriesHandler(
        ICountryRepository repository)
        : IQueryHandler<FilterCountriesQuery, Result<PagedResult<CountryDto>>>
    {
        public async Task<Result<PagedResult<CountryDto>>> Handle(FilterCountriesQuery request, CancellationToken ct)
        {
            var (items, total) = await repository.FilterAsync(request.Filter, ct);
            return Result<PagedResult<CountryDto>>.Success(PagedResult<CountryDto>.From(items, total, request.Filter.PageNumber, request.Filter.PageSize));
        }
    }
}

