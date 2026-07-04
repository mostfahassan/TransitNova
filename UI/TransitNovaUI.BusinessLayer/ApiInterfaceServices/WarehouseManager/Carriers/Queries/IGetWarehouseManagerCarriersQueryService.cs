namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Queries;

public interface IGetWarehouseManagerCarriersQueryService
{
    Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetWarehouseManagerCarriersAsync(Guid warehouseId, UiFilterCarrierDto filter, string bearerToken, CancellationToken cancellationToken = default);
}
