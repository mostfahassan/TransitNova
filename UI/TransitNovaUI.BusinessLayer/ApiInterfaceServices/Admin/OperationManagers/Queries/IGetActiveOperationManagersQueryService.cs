namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetActiveOperationManagersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/operation-managers/active";

    Task<ApiResponse<List<UiOperationManagerProfileDto>>> GetActiveOperationManagersAsync(CancellationToken cancellationToken = default);
}

