namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

public interface IFilterCarriersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-manager/carriers";

    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> FilterCarriersAsync(UiFilterCarrierDto filter, CancellationToken cancellationToken = default);
}

