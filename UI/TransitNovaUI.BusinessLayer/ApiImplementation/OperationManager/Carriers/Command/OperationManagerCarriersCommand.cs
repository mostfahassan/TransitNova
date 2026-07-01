using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Carriers.Command
{
    public class OperationManagerCarriersCommand(IHttpHandler httpHandler, HttpClient httpClient) : IOperationManagerCarriersQuery

    {
        public async Task<ApiResponse> AssignDeliveryCarrierAsync(Guid shipmentId, UiAssignCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiAssignCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.AssignDeliveryCarrierUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> AssignPickupCarrierAsync(Guid shipmentId, UiAssignCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiAssignCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerCarriers.AssignPickupCarrierUrl, ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put,  url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
