namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Queries;

public interface IGetWarehouseManagerDashboardQueryService
{
    Task<ApiResponse<UiWarehouseManagerDashboardDto>> GetWarehouseManagerDashboardAsync(string bearerToken, CancellationToken cancellationToken = default);
}
