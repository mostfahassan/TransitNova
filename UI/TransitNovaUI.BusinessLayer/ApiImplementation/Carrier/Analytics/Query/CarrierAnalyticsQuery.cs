using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Analytics.Query
{
    public class CarrierAnalyticsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ICarrierAnalyticalQuery
    {
        public async Task<ApiResponse<decimal>> GetCarrierRatingAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierAnalytics.GetCarrierRatingUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<decimal>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<decimal>> GetCarrierRevenueAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierAnalytics.GetCarrierRevenueUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<decimal>(httpResponse, cancellationToken);
        }

    }
}
