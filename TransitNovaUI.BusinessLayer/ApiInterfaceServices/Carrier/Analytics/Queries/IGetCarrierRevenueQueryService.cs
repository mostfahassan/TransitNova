namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries;

public interface IGetCarrierRevenueQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/Revenue";

    Task<ApiResponse<decimal>> GetCarrierRevenueAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

