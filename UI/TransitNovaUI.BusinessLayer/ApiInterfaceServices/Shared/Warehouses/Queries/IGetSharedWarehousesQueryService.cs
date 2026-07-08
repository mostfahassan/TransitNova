namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Warehouses.Queries;

public interface IGetSharedWarehousesQueryService
{
    Task<ApiResponse<List<UiWarehouseDto>>> GetWarehousesAsync(string bearerToken, CancellationToken cancellationToken = default);
}