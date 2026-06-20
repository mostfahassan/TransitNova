namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Queries;

public interface IGetHandledCarriersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/{operationManagerId:guid}/handled-carriers";

    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

