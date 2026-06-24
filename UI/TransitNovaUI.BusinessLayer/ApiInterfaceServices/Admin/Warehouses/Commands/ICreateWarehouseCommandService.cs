namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;

public interface ICreateWarehouseCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admin/warehouses";

    Task<ApiResponse<UiWarehouseDto>> CreateWarehouseAsync(UiCreateWarehouseDto request, CancellationToken cancellationToken = default);
}

