using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Warehouses.Query
{
    public class AdminWarehousesQuery(IHttpHandler httpHandler, HttpClient httpClient) :ApiServiceBase(httpHandler, httpClient), IAdminWarehousesQuery
    {
        public Task<ApiResponse<UiWarehouseDto?>> GetWarehouseByIdAsync(Guid warehouseId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.GetWarehouseByIdUrl, ("warehouseId", warehouseId)));

            return SendQueryRequestAsync<UiWarehouseDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<List<UiWarehouseDto>>> GetWarehousesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.GetWarehousesUrl));

            return SendQueryRequestAsync<List<UiWarehouseDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
