using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Shipments.Command
{
    public class UserShipmentsCommand(IHttpHandler httpHandler, HttpClient httpClient) 
        : IUserShipmentCommand
    {
        public async Task<ApiResponse> CancelShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.CancelShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiRetrieveShipmentDto>> CreateShipmentAsync(UiCreateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateShipmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.CreateShipmentUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiRetrieveShipmentDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
      
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.DeleteShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete,url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> IssueShipmentAsync(Guid shipmentId, UiIssueShipmentReason model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiIssueShipmentReason.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.IssueShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch,url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateShipmentAsync(Guid shipmentId, UiUpdateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateShipmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.UpdateShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
