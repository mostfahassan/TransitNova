namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

public interface IDeleteWarehouseCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admin/warehouses/{warehouseId:guid}";

    Task<ApiResponse> DeleteWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
}

