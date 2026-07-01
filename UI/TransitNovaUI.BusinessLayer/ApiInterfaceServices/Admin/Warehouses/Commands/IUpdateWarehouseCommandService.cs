namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

public interface IUpdateWarehouseCommandService
{
    Task<ApiResponse> UpdateWarehouseAsync(Guid warehouseId, UiUpdateWarehouseDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

