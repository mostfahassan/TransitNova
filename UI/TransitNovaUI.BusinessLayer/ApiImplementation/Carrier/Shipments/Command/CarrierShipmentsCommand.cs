using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Shipments.Command
{
    public class CarrierShipmentsCommand(IHttpHandler httpHandler, HttpClient httpClient)
        : ICarrierShipmentCommand
    {
        public async Task<ApiResponse> CompleteDeliveryAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
    
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.CompleteDeliveryUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> CompletePickupAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
       
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.CompletePickupUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateCarrierStatusAsync(Guid carrierId, UiChangeCarrierStatusDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiChangeCarrierStatusDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.UpdateCarrierStatusUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Patch, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);
            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
