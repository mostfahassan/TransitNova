using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Warehouses.Query
{
    public class AdminWarehousesQuery(IHttpHandler httpHandler, HttpClient httpClient) :IAdminWarehousesQuery
    {
        public async Task<ApiResponse<UiWarehouseDto?>> GetWarehouseByIdAsync(Guid warehouseId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.GetWarehouseByIdUrl, ("warehouseId", warehouseId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiWarehouseDto?>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<List<UiWarehouseDto>>> GetWarehousesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.GetWarehousesUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiWarehouseDto>>(httpResponse, cancellationToken);
        }

    }
}
