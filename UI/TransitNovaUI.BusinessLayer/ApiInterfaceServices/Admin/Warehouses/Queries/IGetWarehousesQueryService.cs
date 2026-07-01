namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Queries;

public interface IGetWarehousesQueryService
{
    Task<ApiResponse<List<UiWarehouseDto>>> GetWarehousesAsync(string bearerToken, CancellationToken cancellationToken = default);
}

