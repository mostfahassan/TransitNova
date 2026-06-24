namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;

public interface IGetCarrierShipmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/shipments";

    Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, CancellationToken cancellationToken = default);
}

