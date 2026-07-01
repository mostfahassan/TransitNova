namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Queries;

public interface IGetOperationManagerDashboardQueryService
{
    Task<ApiResponse<UiOperationManagerDashboardDto>> GetOperationManagerDashboardAsync(string bearerToken, CancellationToken cancellationToken = default);
}

