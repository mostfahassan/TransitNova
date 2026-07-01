namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetActiveOperationManagersQueryService
{
    Task<ApiResponse<List<UiOperationManagerProfileDto>>> GetActiveOperationManagersAsync(string bearerToken, CancellationToken cancellationToken = default);
}

