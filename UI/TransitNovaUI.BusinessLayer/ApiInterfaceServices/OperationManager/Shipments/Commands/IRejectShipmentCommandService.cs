namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;

public interface IRejectShipmentCommandService
{
    Task<ApiResponse> RejectShipmentAsync(Guid shipmentId, UiRejectShipmentReason model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

