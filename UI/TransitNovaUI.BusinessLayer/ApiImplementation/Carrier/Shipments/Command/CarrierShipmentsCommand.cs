using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Shipments.Command
{
    public class CarrierShipmentsCommand(IHttpHandler httpHandler, HttpClient httpClient)
        : ApiServiceBase(httpHandler, httpClient), ICarrierShipmentCommand
    {
        public Task<ApiResponse> CompleteDeliveryAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.CompleteDeliveryUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse> CompletePickupAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
       
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.CompletePickupUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse> MarkShipmentPickedUpAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.MarkShipmentPickedUpUrl, ("carrierId", carrierId), ("shipmentId", shipmentId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, null, idempotentKey);
        }
        public Task<ApiResponse> UpdateCarrierStatusAsync(Guid carrierId, UiChangeCarrierStatusDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiChangeCarrierStatusDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierShipments.UpdateCarrierStatusUrl, ("carrierId", carrierId)));

            return SendRequestAsync(HttpMethod.Patch, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
