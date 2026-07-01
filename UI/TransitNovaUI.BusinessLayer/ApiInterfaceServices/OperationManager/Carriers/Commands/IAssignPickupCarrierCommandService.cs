namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;

public interface IAssignPickupCarrierCommandService
{
    Task<ApiResponse> AssignPickupCarrierAsync(Guid shipmentId, UiAssignCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

