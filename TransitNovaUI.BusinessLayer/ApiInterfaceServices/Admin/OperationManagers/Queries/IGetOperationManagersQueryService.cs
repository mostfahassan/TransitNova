namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/operation-managers";

    Task<ApiResponse<IEnumerable<UiOperationManagerProfileDto>>> GetOperationManagersAsync(CancellationToken cancellationToken = default);
}

