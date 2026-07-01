using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Trips.Query
{
    public class OperationManagerTripsQuery(IHttpHandler httpHandler, HttpClient httpClient) : IOperationManagrTripsQuery
    {
        public async Task<ApiResponse<UiCarrierTripDto>> GetCarrierTripByIdAsync(Guid carrierId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerTrips.GetCarrierTripByIdUrl, ("carrierId", carrierId), ("tripId", tripId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierTripDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerTrips.GetCarrierTripsUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get,  url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IReadOnlyCollection<UiCarrierTripDto>>(httpResponse, cancellationToken);
        }

    }
}
