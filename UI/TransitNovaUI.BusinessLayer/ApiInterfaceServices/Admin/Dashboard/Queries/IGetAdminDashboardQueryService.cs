namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Queries;

public interface IGetAdminDashboardQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admins/dashboard";

    Task<ApiResponse<UiAdminDashboardDto>> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}
