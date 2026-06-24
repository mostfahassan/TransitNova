namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Queries;

public interface IGetCarrierDashboardQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/dashboard";

    Task<ApiResponse<UiCarrierDashboardDto>> GetCarrierDashboardAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

