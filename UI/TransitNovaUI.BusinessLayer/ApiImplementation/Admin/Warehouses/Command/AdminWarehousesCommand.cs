using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Warehouses.Command
{
    public class AdminWarehousesCommand(IHttpHandler httpHandler, HttpClient httpClient)
        : ApiServiceBase(httpHandler, httpClient), IAdminWarehousesCommand
    {
        public Task<ApiResponse<UiWarehouseDto>> CreateWarehouseAsync(UiCreateWarehouseDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateWarehouseDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.CreateWarehouseUrl));

            return SendRequestAsync<UiWarehouseDto>(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteWarehouseAsync(Guid warehouseId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.DeleteWarehouseUrl, ("warehouseId", warehouseId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse> UpdateWarehouseAsync(Guid warehouseId, UiUpdateWarehouseDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateWarehouseDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.UpdateWarehouseUrl, ("warehouseId", warehouseId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
