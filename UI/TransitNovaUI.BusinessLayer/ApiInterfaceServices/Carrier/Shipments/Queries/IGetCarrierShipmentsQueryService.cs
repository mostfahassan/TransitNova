namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Queries;

public interface IGetCarrierShipmentsQueryService
{
    Task<ApiResponse<UiCarrierShipmentListDto>> GetCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

