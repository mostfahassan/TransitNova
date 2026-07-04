using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.CarrierRatings.Command
{
    public class UserCarrierRatingsCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserRatingCommand
    {
        public Task<ApiResponse> RateDeliveryCarrierAsync(Guid shipmentId, UiRatingCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRatingCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserCarrierRatings.RateDeliveryCarrierUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> RatePickupCarrierAsync(Guid shipmentId, UiRatingCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRatingCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserCarrierRatings.RatePickupCarrierUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
