namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetShipmentByIdQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> GetShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

