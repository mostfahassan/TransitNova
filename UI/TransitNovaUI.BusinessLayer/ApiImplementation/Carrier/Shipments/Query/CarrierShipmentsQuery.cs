using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Shipments.Query
{
    public class CarrierShipmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ICarrierShipmentQuery
    {
        public Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.GetCarrierShipmentByIdUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            return SendQueryRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
         
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.GetCarrierShipmentsUrl, ("carrierId", carrierId),
                ("Status", filter.Status), ("Mode", filter.Mode), ("SearchTerm", filter.SearchTerm),
                ("SortBy", filter.SortBy), ("SortDescending", filter.SortDescending),
                ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiCarrierShipmentListDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
