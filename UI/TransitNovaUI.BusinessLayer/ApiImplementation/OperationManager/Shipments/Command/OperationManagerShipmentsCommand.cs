using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Shipments.Command
{
    public class OperationManagerShipmentsCommand(IHttpHandler httpHandler, HttpClient httpClient): ApiServiceBase(httpHandler, httpClient), IOperationManagerShipmentsCommand
    {
        public Task<ApiResponse> ApproveShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.ApproveShipmentUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse> RejectShipmentAsync(Guid shipmentId, UiRejectShipmentReason model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRejectShipmentReason.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerShipments.RejectShipmentUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
