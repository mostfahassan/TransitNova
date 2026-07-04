namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Shipments.Queries;

public interface IGetWarehouseManagerShipmentsQueryService
{
    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetWarehouseManagerShipmentsAsync(Guid warehouseId, UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}
