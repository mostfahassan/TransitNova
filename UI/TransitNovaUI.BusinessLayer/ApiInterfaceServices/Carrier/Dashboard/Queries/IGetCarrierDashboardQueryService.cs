namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Queries;

public interface IGetCarrierDashboardQueryService
{
    Task<ApiResponse<UiCarrierDashboardDto>> GetCarrierDashboardAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

