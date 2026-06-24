using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;


namespace TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository
{
    public interface ICityRepository
     : IGenericRepository<City, int>,
       IBaseLocationRepository<City, CityDto, CityFilterDto>
    {

        Task<bool> NameExistsForAnotherGovernmentAsync(int governmentId, string name, CancellationToken ct);
    }
}

