namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

public interface ICreateWarehouseCommandService
{
    Task<ApiResponse<UiWarehouseDto>> CreateWarehouseAsync(UiCreateWarehouseDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

