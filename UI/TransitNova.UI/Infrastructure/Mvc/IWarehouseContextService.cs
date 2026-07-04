namespace TransitNova.UI.Infrastructure.Mvc;

public interface IWarehouseContextService
{
    Task<Guid?> GetWarehouseIdAsync(CancellationToken cancellationToken = default);
}
