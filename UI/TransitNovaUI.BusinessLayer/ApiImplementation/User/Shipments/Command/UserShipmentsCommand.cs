using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Shipments.Command
{
    public class UserShipmentsCommand(IHttpHandler httpHandler, HttpClient httpClient) 
        : ApiServiceBase(httpHandler, httpClient), IUserShipmentCommand
    {
        public Task<ApiResponse> CancelShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.CancelShipmentUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse<UiRetrieveShipmentDto>> CreateShipmentAsync(UiCreateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateShipmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.CreateShipmentUrl));

            return SendRequestAsync<UiRetrieveShipmentDto>(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
      
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.DeleteShipmentUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse> IssueShipmentAsync(Guid shipmentId, UiIssueShipmentReason model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiIssueShipmentReason.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.IssueShipmentUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> UpdateShipmentAsync(Guid shipmentId, UiUpdateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateShipmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserShipments.UpdateShipmentUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
