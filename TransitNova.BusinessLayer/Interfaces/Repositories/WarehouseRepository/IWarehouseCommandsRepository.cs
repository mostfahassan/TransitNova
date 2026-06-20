using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseRepository
{
    public interface IWarehouseCommandsRepository:IGenericRepository<Warehouse,Guid>
    {

    }
    public interface IWarehouseRulesRepository:IGenericRepository<Warehouse,Guid>
    {
        Task<bool> ExistsByNameAsync(string name, Guid? excludedWarehouseId, CancellationToken ct);

    }

}
