namespace TransitNova.BusinessLayer.Interfaces.Repositories.WarehouseManagerRepository
{
    public interface IWarehouseManagerRuleseRepository
    {
        Task<bool> ExistsAsync(Guid managerId, CancellationToken ct = default);
        Task <bool> IsWarehouseManager (Guid managerId, Guid warehouseId , CancellationToken ct = default);
    }
}

