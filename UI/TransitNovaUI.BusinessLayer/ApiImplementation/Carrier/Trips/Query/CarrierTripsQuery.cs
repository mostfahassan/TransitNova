using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Trips.Query
{
    public class CarrierTripsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ICarrierTripsQuery
    {
        public async Task<ApiResponse<UiCarrierTripDto>> GetCarrierTripByIdAsync(Guid carrierId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierTrips.GetCarrierTripByIdUrl, ("carrierId", carrierId), ("tripId", tripId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierTripDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierTrips.GetCarrierTripsUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IReadOnlyCollection<UiCarrierTripDto>>(httpResponse, cancellationToken);
        }

    }
}
