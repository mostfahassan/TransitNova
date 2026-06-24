namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Queries;

public interface IGetWarehousesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/warehouses";

    Task<ApiResponse<List<UiWarehouseDto>>> GetWarehousesAsync(CancellationToken cancellationToken = default);
}

