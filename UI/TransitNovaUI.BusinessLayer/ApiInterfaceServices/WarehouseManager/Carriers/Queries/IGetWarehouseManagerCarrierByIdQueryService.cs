namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Queries;

public interface IGetWarehouseManagerCarrierByIdQueryService
{
    Task<ApiResponse<UiCarrierProfileDto>> GetWarehouseManagerCarrierByIdAsync(Guid warehouseId, Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}
