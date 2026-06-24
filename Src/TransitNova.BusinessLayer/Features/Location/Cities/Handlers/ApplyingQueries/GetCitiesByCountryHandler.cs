using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyingQueries
{
    public sealed class GetCitiesByCountryHandler(
        ICityRepository repository)

        : IQueryHandler<GetCitiesByCountryQuery, Result<IEnumerable<CityDto>>>
    {
        public async Task<Result<IEnumerable<CityDto>>> Handle(GetCitiesByCountryQuery request, CancellationToken ct)
        {
            var cities = await repository.GetAllAsync(ct, request.GovernmentId);
            return Result<IEnumerable<CityDto>>.Success(cities);
        }
    }
}

