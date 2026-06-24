using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;

namespace TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyingQueries
{
    public class GetCitiesByGovernmentHandler(ICityRepository cityRepository) : IQueryHandler<GetCitiesByGovernmentQuery, Result<IEnumerable<CityDto>>>
    {
        public async Task<Result<IEnumerable<CityDto>>> Handle(GetCitiesByGovernmentQuery request, CancellationToken cancellationToken)
        {
            var cities = await cityRepository.GetAllAsync(cancellationToken, request.GovernmentId);
            if (!cities.Any())
            {
                var emptyResult = Result<IEnumerable<CityDto>>.Success([]);
                return emptyResult;
            }

            var result = Result<IEnumerable<CityDto>>.Success(cities);
            return result;
        }
    }
}
