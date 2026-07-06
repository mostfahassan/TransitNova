namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface IMarkShipmentPickedUpCommandService
{
    Task<ApiResponse> MarkShipmentPickedUpAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}
