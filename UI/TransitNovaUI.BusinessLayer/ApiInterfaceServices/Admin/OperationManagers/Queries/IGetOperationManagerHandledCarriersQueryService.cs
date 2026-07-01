namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagerHandledCarriersQueryService
{
    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

