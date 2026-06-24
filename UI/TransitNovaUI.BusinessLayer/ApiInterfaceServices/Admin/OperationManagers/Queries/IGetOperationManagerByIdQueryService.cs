namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagerByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/operation-managers/{operationManagerId:guid}";

    Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, CancellationToken cancellationToken = default);
}

