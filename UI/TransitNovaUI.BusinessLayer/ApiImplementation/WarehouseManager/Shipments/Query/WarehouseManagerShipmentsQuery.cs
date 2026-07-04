using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Shipments.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Shipments.Query
{
    public class WarehouseManagerShipmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IWarehouseManagerShipmentsQuery
    {
        public Task<ApiResponse<UiRetrieveShipmentDto>> GetWarehouseManagerShipmentByIdAsync(Guid warehouseId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerShipments.GetShipmentByIdUrl, ("shipmentId", shipmentId), ("warehouseId", warehouseId)));

            return SendQueryRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetWarehouseManagerShipmentsAsync(Guid warehouseId, UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerShipments.GetShipmentsUrl, ("warehouseId", warehouseId), ("Status", filter.Status), ("Mode", filter.Mode), ("From", filter.From), ("To", filter.To), ("SenderId", filter.SenderId), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiRetrieveShipmentDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}
