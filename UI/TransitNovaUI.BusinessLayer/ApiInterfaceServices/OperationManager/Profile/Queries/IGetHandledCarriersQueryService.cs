namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;

public interface IGetHandledCarriersQueryService
{
    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

