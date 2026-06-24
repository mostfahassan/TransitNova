using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyingQueries
{
    public sealed class GetCityByIdHandler(ICityRepository repository)
        : IQueryHandler<GetCityByIdQuery, Result<CityDto?>>
    {
        public async Task<Result<CityDto?>> Handle(GetCityByIdQuery request, CancellationToken ct)
        {
            var dto = await repository.GetByIdAsync<CityDto>(request.Id, ct);
            var result = dto is null ? Result<CityDto?>.NotFound(Errors.NotFound("City not found.")) : Result<CityDto?>.Success(dto);
            return result;
        }
    }
}

