namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries;

public interface IGetCarrierRatingQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/rating";

    Task<ApiResponse<decimal>> GetCarrierRatingAsync(Guid carrierId, CancellationToken cancellationToken = default);
}
