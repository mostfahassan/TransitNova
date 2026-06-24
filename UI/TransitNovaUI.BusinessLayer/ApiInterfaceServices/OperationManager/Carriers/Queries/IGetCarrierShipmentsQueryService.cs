namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

public interface IGetCarrierShipmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-managers/carriers/{carrierId:guid}/shipments";

    Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, CancellationToken cancellationToken = default);
}
