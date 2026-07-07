using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Shipment.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Shipments.Query
{
    internal class AdminShipmentQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminShipmentsQuery
    {
        public Task<ApiResponse<UiRetrieveShipmentDto>> GetAdminShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminShipments.GetShipmentDetailsUrl, ("shipmentId", shipmentId)));
            return SendQueryRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Get, url, bearerToken, cancellationToken, null, null);
        }

        public Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetAdminShipmentsAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminShipments.FilterShipmentsUrl,
                ("Status", filter.Status),
                ("Mode", filter.Mode),
                ("From", filter.From),
                ("To", filter.To),
                ("SenderId", filter.SenderId),
                ("SearchTerm", filter.SearchTerm),
                ("PageNumber", filter.PageNumber),
                ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiRetrieveShipmentDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, null, null);
        }
    }
}