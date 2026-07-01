using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Shipments.Query
{
    public class CarrierShipmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ICarrierShipmentQuery
    {
        public async Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.GetCarrierShipmentByIdUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRetrieveShipmentDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
         
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.GetCarrierShipmentsUrl, ("carrierId", carrierId),
                ("Status", filter.Status), ("Mode", filter.Mode), ("SearchTerm", filter.SearchTerm),
                ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending),
                ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierShipmentListDto>(httpResponse, cancellationToken);
        }

    }
}
