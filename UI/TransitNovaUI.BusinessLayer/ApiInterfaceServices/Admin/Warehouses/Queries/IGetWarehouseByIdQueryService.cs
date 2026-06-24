namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Queries;

public interface IGetWarehouseByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/warehouses/{warehouseId:guid}";

    Task<ApiResponse<UiWarehouseDto?>> GetWarehouseByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
}

