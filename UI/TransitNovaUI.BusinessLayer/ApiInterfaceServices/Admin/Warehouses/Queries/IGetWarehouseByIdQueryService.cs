namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Queries;

public interface IGetWarehouseByIdQueryService
{
    Task<ApiResponse<UiWarehouseDto?>> GetWarehouseByIdAsync(Guid warehouseId, string bearerToken, CancellationToken cancellationToken = default);
}

