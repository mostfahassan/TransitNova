namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;

public interface IApproveShipmentCommandService
{
    Task<ApiResponse> ApproveShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

