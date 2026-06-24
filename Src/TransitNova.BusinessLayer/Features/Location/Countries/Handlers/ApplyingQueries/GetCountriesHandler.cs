using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
namespace TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyingQueries
{
    public class GetCountriesHandler(ICountryRepository countryRepository) : IQueryHandler<GetCountriesQuery, Result<IEnumerable<CountryDto>>>
    {
        public async Task<Result<IEnumerable<CountryDto>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await countryRepository.GetListAsync<CountryDto>(cancellationToken);
            return Result<IEnumerable<CountryDto>>.Success(countries);
        }
    }
}
