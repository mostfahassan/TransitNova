using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Warehouses.Command
{
    public class AdminWarehousesCommand(IHttpHandler httpHandler, HttpClient httpClient)
        : IAdminWarehousesCommand
    {
        public async Task<ApiResponse<UiWarehouseDto>> CreateWarehouseAsync(UiCreateWarehouseDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateWarehouseDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.CreateWarehouseUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post,  url, bearerToken, cancellationToken, content, idempotentKey);
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiWarehouseDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteWarehouseAsync(Guid warehouseId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.DeleteWarehouseUrl, ("warehouseId", warehouseId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateWarehouseAsync(Guid warehouseId, UiUpdateWarehouseDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateWarehouseDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Warehouses.UpdateWarehouseUrl, ("warehouseId", warehouseId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
