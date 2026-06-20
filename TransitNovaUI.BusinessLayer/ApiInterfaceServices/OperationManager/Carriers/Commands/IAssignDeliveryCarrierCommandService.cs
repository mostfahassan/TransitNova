namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Commands;

public interface IAssignDeliveryCarrierCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/operation-manager/carriers/{shipmentId:guid}/assign-delivery";

    Task<ApiResponse> AssignDeliveryCarrierAsync(Guid shipmentId, UiAssignCarrierDto request, CancellationToken cancellationToken = default);
}

