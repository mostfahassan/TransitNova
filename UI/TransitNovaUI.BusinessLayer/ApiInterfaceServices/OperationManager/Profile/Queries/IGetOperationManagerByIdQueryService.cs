namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;

public interface IGetOperationManagerByIdQueryService
{
    Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, string bearerToken, CancellationToken cancellationToken = default);
}

