namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetShipmentHistoriesQueryService
{
    Task<ApiResponse<IEnumerable<UiRetrieveShipmentStatusDto>>> GetShipmentHistoriesAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

