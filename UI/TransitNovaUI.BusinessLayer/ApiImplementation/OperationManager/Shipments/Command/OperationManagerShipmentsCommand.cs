using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Shipments.Command
{
    public class OperationManagerShipmentsCommand(IHttpHandler httpHandler, HttpClient httpClient): IOperationManagerShipmentsCommand
    {
        public async Task<ApiResponse> ApproveShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.ApproveShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> RejectShipmentAsync(Guid shipmentId, UiRejectShipmentReason model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRejectShipmentReason.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.RejectShipmentUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch,  url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
