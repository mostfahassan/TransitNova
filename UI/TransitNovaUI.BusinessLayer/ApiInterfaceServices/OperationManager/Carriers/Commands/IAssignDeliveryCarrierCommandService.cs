namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;

public interface IAssignDeliveryCarrierCommandService
{
    Task<ApiResponse> AssignDeliveryCarrierAsync(Guid shipmentId, UiAssignCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

