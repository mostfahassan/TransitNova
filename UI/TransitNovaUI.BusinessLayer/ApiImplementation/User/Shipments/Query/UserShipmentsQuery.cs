using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Shipments.Query
{
    public class UserShipmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) : IUserShipmentQuery
    {
        public async Task<ApiResponse<UiRetrieveShipmentDto>> GetUserShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.GetUserShipmentByIdUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRetrieveShipmentDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiRetrieveShipmentSummaryDto>> TrackShipmentAsync(string trackingNumber, string bearerToken, CancellationToken cancellationToken = default)
        {
         
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.TrackShipmentUrl, ("trackingNumber", trackingNumber)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRetrieveShipmentSummaryDto>(httpResponse, cancellationToken);
        }

    }
}
