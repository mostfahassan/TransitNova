namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;

public interface IAssignPickupCarrierCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/operation-managers/carriers/{shipmentId:guid}/assign-pickup";

    Task<ApiResponse> AssignPickupCarrierAsync(Guid shipmentId, UiAssignCarrierDto request, CancellationToken cancellationToken = default);
}
