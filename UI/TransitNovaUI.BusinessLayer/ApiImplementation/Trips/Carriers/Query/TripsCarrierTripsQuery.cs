using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Trips.Carriers.Query
{
    public class TripsCarrierTripsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ITripsCarrierTripsQuery
    {
        public Task<ApiResponse<UiCarrierTripDto>> GetCarrierTripByIdAsync(Guid carrierId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Trips.GetCarrierTripByIdUrl, ("carrierId", carrierId), ("tripId", tripId)));

            return SendQueryRequestAsync<UiCarrierTripDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Trips.GetCarrierTripsUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<IReadOnlyCollection<UiCarrierTripDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}
