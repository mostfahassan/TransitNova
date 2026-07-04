namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Shipments.Queries;

public interface IGetWarehouseManagerShipmentByIdQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> GetWarehouseManagerShipmentByIdAsync(Guid warehouseId, Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}
