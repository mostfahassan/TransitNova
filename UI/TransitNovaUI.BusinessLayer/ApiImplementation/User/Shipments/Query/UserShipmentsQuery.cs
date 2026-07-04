using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Shipments.Query
{
    public class UserShipmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserShipmentQuery
    {
        public Task<ApiResponse<UiRetrieveShipmentDto>> GetUserShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.GetUserShipmentByIdUrl, ("shipmentId", shipmentId)));

            return SendQueryRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiRetrieveShipmentSummaryDto>> TrackShipmentAsync(string trackingNumber, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.TrackShipmentUrl, ("trackingNumber", trackingNumber)));

            return SendQueryRequestAsync<UiRetrieveShipmentSummaryDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
