namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;

public interface IGetCarrierShipmentByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/shipments/{shipmentId:guid}";

    Task<ApiResponse<UiRetrieveShipmentDto>> GetCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, CancellationToken cancellationToken = default);
}

