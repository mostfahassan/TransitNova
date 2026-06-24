using FluentValidation;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyingQueries
{
    public sealed class FilterCitiesHandler(ICityRepository repository)
        : IQueryHandler<FilterCitiesQuery, Result<PagedResult<CityDto>>>
    {
        public async Task<Result<PagedResult<CityDto>>> Handle(FilterCitiesQuery request, CancellationToken ct)
        {
            var (items, total) = await repository.FilterAsync(request.Filter, ct);
            var result = Result<PagedResult<CityDto>>.Success(PagedResult<CityDto>.From(items, total, request.Filter.PageNumber, request.Filter.PageSize));
            return result;
        }
    }
}

