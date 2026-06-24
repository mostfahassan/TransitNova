using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Features.Zones.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyQueries
{
    public sealed class GetZoneByIdHandler(IZoneRepository repository)
        : IQueryHandler<GetZoneByIdQuery, Result<ZoneDto?>>
    {
        public async Task<Result<ZoneDto?>> Handle(GetZoneByIdQuery request, CancellationToken ct)
        {
            var dto = await repository.GetByIdAsync<ZoneDto>(request.Id, ct);
            return dto is null ? Result<ZoneDto?>.NotFound(Errors.ZoneNotFound("Zone not found.")) : Result<ZoneDto?>.Success(dto);
        }
    }
}

