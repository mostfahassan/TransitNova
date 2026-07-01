namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagersQueryService
{
    Task<ApiResponse<IEnumerable<UiOperationManagerProfileDto>>> GetOperationManagersAsync(string bearerToken, CancellationToken cancellationToken = default);
}

