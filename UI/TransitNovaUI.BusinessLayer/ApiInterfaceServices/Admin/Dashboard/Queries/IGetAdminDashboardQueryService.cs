namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Queries;

public interface IGetAdminDashboardQueryService
{
    Task<ApiResponse<UiAdminDashboardDto>> GetAdminDashboardAsync(string bearerToken, CancellationToken cancellationToken = default);
}

