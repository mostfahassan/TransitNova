namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Queries;

public interface IGetAdminCarriersQueryService
{
    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetAdminCarriersAsync(UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

