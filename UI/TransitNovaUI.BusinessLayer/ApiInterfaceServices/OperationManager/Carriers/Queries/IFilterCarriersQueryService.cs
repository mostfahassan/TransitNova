namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

public interface IFilterCarriersQueryService
{
    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> FilterCarriersAsync(UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

