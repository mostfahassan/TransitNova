using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Subscriptions.Query
{
    public class AdminSubscriptionsQuery(IHttpHandler httpHandler, HttpClient httpClient) : IAdminSubscriptionQuery
    {
        public async Task<ApiResponse<List<UiUserProfileDto>>> GetBundleSubscribersAsync(int bundleId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Subscriptions.GetBundleSubscribersUrl, ("bundleId", bundleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiUserProfileDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiBundleSubscriptionDetailsDto>> GetSubscriptionByIdAsync(Guid subscriptionId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Subscriptions.GetSubscriptionByIdUrl, ("subscriptionId", subscriptionId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiBundleSubscriptionDetailsDto>(httpResponse, cancellationToken);
        }

    }
}
