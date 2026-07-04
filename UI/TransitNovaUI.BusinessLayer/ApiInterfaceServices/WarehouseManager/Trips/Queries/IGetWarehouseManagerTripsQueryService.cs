namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Trips.Queries;

public interface IGetWarehouseManagerTripsQueryService
{
    Task<ApiResponse<UiPagedResult<UiTripDetailsDto>>> GetWarehouseManagerTripsAsync(Guid warehouseId, UiFilterTripsDto filter, string bearerToken, CancellationToken cancellationToken = default);
}
