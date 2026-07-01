namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;

public interface IGetCarrierShipmentByIdQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

