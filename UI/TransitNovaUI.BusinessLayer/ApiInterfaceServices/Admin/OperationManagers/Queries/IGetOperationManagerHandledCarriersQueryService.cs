namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Queries;

public interface IGetOperationManagerHandledCarriersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/operation-managers/{operationManagerId:guid}/handled-carriers";

    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

