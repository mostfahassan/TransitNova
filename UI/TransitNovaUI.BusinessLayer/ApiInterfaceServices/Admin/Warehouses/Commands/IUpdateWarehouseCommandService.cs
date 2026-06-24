namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

public interface IUpdateWarehouseCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admin/warehouses/{warehouseId:guid}";

    Task<ApiResponse> UpdateWarehouseAsync(Guid warehouseId, UiUpdateWarehouseDto request, CancellationToken cancellationToken = default);
}

