namespace TransitNova.UI.Infrastructure.Mvc.Interface;

public interface IWarehouseContextService
{
    Task<Guid?> GetWarehouseIdAsync(CancellationToken cancellationToken = default);
}
