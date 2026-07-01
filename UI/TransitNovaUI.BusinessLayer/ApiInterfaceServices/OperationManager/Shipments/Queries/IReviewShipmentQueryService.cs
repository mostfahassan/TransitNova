namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IReviewShipmentQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> ReviewShipmentAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

