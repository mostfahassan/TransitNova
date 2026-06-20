namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IReviewShipmentQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-manager/shipments/{shipmentId:guid}/review";

    Task<ApiResponse<UiRetrieveShipmentDto>> ReviewShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}

