using FluentValidation;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Features.Zones.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyQueries
{
    public sealed class GetZonesByCityHandler(IZoneRepository repository)
        : IQueryHandler<GetZonesByCityQuery, Result<PagedResult<ZoneDto>>>
    {
        public async Task<Result<PagedResult<ZoneDto>>> Handle(GetZonesByCityQuery request, CancellationToken ct)
        {

            var (items, total) = await repository.GetByCityIdAsync(request.CityId, request.Filter, ct);
            return Result<PagedResult<ZoneDto>>.Success(PagedResult<ZoneDto>.From(items, total, request.Filter.PageNumber, request.Filter.PageSize));
        }
    }
}

