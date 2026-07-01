using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Subscriptions.Command
{
    public class UserSubscriptionsCommand(IHttpHandler httpHandler, HttpClient httpClient) : IUserSubscriptionCommand
    {
        public async Task<ApiResponse> SubscribeToBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserSubscriptions.SubscribeToBundleUrl, ("bundleId", bundleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UnsubscribeFromBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserSubscriptions.UnsubscribeFromBundleUrl, ("bundleId", bundleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
