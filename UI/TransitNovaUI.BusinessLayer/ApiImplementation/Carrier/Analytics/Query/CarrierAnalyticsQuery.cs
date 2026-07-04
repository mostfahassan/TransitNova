using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Analytics.Query
{
    public class CarrierAnalyticsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ICarrierAnalyticalQuery
    {
        public Task<ApiResponse<decimal>> GetCarrierRatingAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierAnalytics.GetCarrierRatingUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<decimal>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<decimal>> GetCarrierRevenueAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierAnalytics.GetCarrierRevenueUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<decimal>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
