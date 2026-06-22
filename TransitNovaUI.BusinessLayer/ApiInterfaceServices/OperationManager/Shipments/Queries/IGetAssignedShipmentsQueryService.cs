namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetAssignedShipmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/shipments/assigned";

    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetAssignedShipmentsAsync(UiShipmentFilterDto filter, CancellationToken cancellationToken = default);
}
