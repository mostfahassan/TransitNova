namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Queries;

public interface IGetOperationManagerDashboardQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-manager/dashboard";

    Task<ApiResponse<UiOperationManagerDashboardDto>> GetOperationManagerDashboardAsync(CancellationToken cancellationToken = default);
}

