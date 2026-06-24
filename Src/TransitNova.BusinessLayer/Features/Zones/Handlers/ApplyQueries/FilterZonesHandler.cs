using FluentValidation;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Features.Zones.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyQueries
{
    public sealed class FilterZonesHandler(IZoneRepository repository)
        : IQueryHandler<FilterZonesQuery, Result<PagedResult<ZoneDto>>>
    {
        public async Task<Result<PagedResult<ZoneDto>>> Handle(FilterZonesQuery request, CancellationToken ct)
        {
            var (items, total) = await repository.FilterAsync(request.Filter, ct);
            var result = Result<PagedResult<ZoneDto>>.Success(PagedResult<ZoneDto>.From(items, total, request.Filter.PageNumber, request.Filter.PageSize));
            return result;
        }
    }
}

