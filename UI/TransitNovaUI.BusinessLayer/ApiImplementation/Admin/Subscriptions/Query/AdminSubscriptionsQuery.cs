using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Subscriptions.Query
{
    public class AdminSubscriptionsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminSubscriptionQuery
    {
        public Task<ApiResponse<List<UiBundleSubscriptionDetailsDto>>> GetSubscribersAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Subscriptions.GetSubscribersUrl));

            return SendQueryRequestAsync<List<UiBundleSubscriptionDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }


        public Task<ApiResponse<List<UiBundleSubscriptionDetailsDto>>> GetBundleSubscribersAsync(Guid bundleId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Subscriptions.GetBundleSubscribersUrl, ("bundleId", bundleId)));

            return SendQueryRequestAsync<List<UiBundleSubscriptionDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
        public Task<ApiResponse<UiBundleSubscriptionDetailsDto>> GetSubscriptionByIdAsync(Guid subscriptionId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Subscriptions.GetSubscriptionByIdUrl, ("subscriptionId", subscriptionId)));

            return SendQueryRequestAsync<UiBundleSubscriptionDetailsDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
