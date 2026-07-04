namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Queries;

public interface IGetAdminCarrierShipmentsQueryService
{
    Task<ApiResponse<UiCarrierShipmentListDto>> GetAdminCarrierShipmentsAsync(Guid carrierId, UiCarrierShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

