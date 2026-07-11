using TransitNovaUI.BusinessLayer.DTOs.Payment;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Subscriptions.Command
{
    public class UserSubscriptionsCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserSubscriptionCommand
    {
        public Task<ApiResponse<UiBundleInvoiceDto>> SubscribeToBundleAsync(Guid bundleId, UiSubscribeToBundleDto dto, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserSubscriptions.SubscribeToBundleUrl, ("bundleId", bundleId)));

            return SendRequestAsync<UiBundleInvoiceDto>(HttpMethod.Post, url, bearerToken, cancellationToken, dto, idempotentKey);
        }

        public Task<ApiResponse> UnsubscribeFromBundleAsync(Guid bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserSubscriptions.UnsubscribeFromBundleUrl, ("bundleId", bundleId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken, null, idempotentKey);
        }

    }
}



