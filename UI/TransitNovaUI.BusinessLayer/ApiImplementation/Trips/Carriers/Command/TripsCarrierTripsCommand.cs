using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Commands;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Trips.Carriers.Command
{
    public class TripsCarrierTripsCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ITripsCarrierTripsCommand
    {
        public Task<ApiResponse> CompleteCarrierTripAsync(Guid carrierId, Guid tripId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Trips.CompleteCarrierTripUrl, ("carrierId", carrierId), ("tripId", tripId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, null, idempotentKey);
        }
    }
}