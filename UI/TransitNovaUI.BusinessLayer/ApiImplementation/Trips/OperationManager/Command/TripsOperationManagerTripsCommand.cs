using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Segregations.Commands;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Trips.OperationManager.Command
{
    public class TripsOperationManagerTripsCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ITripsOperationManagerTripsCommand
    {
        public Task<ApiResponse> StartDeliveryTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {

            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Trips.StartDeliveryTripUrl, ("carrierId", carrierId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, null, idempotentKey);
        }

        public Task<ApiResponse> StartPickupTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
      
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Trips.StartPickupTripUrl, ("carrierId", carrierId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, null, idempotentKey);
        }
    }
}
