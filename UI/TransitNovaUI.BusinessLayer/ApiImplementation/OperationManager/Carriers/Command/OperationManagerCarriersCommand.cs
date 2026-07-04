using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Carriers.Command
{
    public class OperationManagerCarriersCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IOperationManagerCarriersCommand

    {
        public Task<ApiResponse> AssignDeliveryCarrierAsync(Guid shipmentId, UiAssignCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiAssignCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.AssignDeliveryCarrierUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> AssignPickupCarrierAsync(Guid shipmentId, UiAssignCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiAssignCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.AssignPickupCarrierUrl, ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}

