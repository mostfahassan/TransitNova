namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface ICompletePickupCommandService
{
    Task<ApiResponse> CompletePickupAsync(Guid carrierId, Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

