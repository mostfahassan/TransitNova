namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Commands;

public interface IApproveShipmentCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/operation-manager/shipments/{shipmentId:guid}/approve";

    Task<ApiResponse> ApproveShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}

