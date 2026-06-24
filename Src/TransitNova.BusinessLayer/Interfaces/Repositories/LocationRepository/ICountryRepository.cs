using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository
{
    public interface ICountryRepository
    : IGenericRepository<Country, int>,
      IBaseLocationRepository<Country, CountryDto, CountryFilterDto>
    {
        Task<IEnumerable<GovernmentDto>> GetAllGovernmentsAsync(int countryId, CancellationToken ct);
    }
}

