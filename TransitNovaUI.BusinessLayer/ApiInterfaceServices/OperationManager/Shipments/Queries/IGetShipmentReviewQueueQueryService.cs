namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetShipmentReviewQueueQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-manager/shipments/review-queue";

    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetShipmentReviewQueueAsync(UiShipmentFilterDto filter, CancellationToken cancellationToken = default);
}

