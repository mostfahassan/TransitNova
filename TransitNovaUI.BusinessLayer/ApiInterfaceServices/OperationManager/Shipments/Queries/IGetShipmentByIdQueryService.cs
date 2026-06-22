namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetShipmentByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/shipments/{shipmentId:guid}";

    Task<ApiResponse<UiRetrieveShipmentDto>> GetShipmentByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}
