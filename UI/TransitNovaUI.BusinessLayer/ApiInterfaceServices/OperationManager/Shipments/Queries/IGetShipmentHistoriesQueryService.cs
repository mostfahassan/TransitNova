namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetShipmentHistoriesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/shipments/{shipmentId:guid}/histories";

    Task<ApiResponse<IEnumerable<UiRetrieveShipmentStatusDto>>> GetShipmentHistoriesAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}
