using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Trips.Command
{
    public class OperationManagerTripsCommand(IHttpHandler httpHandler, HttpClient httpClient) : IOperationManagrTripsCommand
    {
        public async Task<ApiResponse> StartDeliveryTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {  
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerTrips.StartDeliveryTripUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> StartPickupTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
        
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerTrips.StartPickupTripUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch,url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
