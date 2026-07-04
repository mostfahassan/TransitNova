namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Trips.Queries;

public interface IGetWarehouseManagerTripByIdQueryService
{
    Task<ApiResponse<UiTripDetailsDto>> GetWarehouseManagerTripByIdAsync(Guid warehouseId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default);
}
