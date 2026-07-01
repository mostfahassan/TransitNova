namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

public interface IGetCarrierShipmentByIdQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

