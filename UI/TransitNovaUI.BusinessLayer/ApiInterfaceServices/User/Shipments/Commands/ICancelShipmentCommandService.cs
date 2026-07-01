namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface ICancelShipmentCommandService
{
    Task<ApiResponse> CancelShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

