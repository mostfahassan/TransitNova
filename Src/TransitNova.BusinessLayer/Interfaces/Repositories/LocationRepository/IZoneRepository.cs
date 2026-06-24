using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository
{
    public interface IZoneRepository:IGenericRepository<Zone, Guid>
    {
       
        Task<(List<ZoneDto> Items, int TotalCount)> FilterAsync(ZoneFilterDto filter, CancellationToken ct);
        Task<(List<ZoneDto> Items, int TotalCount)> GetByCityIdAsync(int cityId, ZoneFilterDto filter, CancellationToken ct);
        Task<bool> NameExistsInCityAsync(int cityId, string name, CancellationToken ct);
        Task<bool> CodeExistsInCityAsync(int cityId, string code, CancellationToken ct);
        Task<bool> NameExistsInCityForAnotherAsync(Guid zoneId, int cityId, string name, CancellationToken ct);
        Task<bool> CodeExistsInCityForAnotherAsync(Guid zoneId, int cityId, string code, CancellationToken ct);
        Task<bool> CityExistsAsync(int cityId, CancellationToken ct);
    }
}

