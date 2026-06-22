namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;

public interface IRejectShipmentCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/operation-managers/shipments/{shipmentId:guid}/reject";

    Task<ApiResponse> RejectShipmentAsync(Guid shipmentId, UiRejectShipmentReason request, CancellationToken cancellationToken = default);
}
