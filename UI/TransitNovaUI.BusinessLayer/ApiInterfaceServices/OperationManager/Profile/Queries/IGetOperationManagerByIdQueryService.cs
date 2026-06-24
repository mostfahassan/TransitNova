namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;

public interface IGetOperationManagerByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/{operationManagerId:guid}";

    Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, CancellationToken cancellationToken = default);
}

