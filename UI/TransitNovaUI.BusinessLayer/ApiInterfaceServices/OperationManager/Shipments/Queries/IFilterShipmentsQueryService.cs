namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IFilterShipmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/shipments";

    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> FilterShipmentsAsync(UiShipmentFilterDto filter, CancellationToken cancellationToken = default);
}
