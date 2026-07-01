namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetShipmentReviewQueueQueryService
{
    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetShipmentReviewQueueAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

