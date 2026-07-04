namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Queries;

public interface IGetAdminCarrierShipmentByIdQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> GetAdminCarrierShipmentByIdAsync(Guid carrierId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

