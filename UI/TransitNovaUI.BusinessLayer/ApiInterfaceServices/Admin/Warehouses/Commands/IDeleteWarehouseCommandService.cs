namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

public interface IDeleteWarehouseCommandService
{
    Task<ApiResponse> DeleteWarehouseAsync(Guid warehouseId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

