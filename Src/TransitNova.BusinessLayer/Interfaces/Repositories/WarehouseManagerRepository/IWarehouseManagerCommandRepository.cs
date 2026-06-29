using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository
{
    public interface IWarehouseManagerCommandRepository
    {
        Task<WarehouseManagerProfile?> GetByIdForUpdateAsync(Guid managerId, CancellationToken ct = default);
    }
}
