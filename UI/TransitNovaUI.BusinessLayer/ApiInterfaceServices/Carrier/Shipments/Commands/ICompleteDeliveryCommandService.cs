namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface ICompleteDeliveryCommandService
{
    Task<ApiResponse> CompleteDeliveryAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

