using TransitNova.BusinessLayer.DTOs.Vehicle;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.VehicleRepository
{
    public interface IVehicleQueryRepository : IGenericRepository<Vehicle, Guid>
    {
        Task<List<VehicleDto>> GetActiveAsync(CancellationToken ct);
        Task<VehicleDto?> GetByCarrierIdAsync(Guid carrierId, CancellationToken ct);
        Task<VehicleDto?> GetVehicleDetailsAsync(Guid id, CancellationToken ct);
        Task<VehicleDto?> GetByPlateNumberAsync(string plateNumber, CancellationToken ct);
    }
}
