namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Queries;

public interface IGetOperationManagerTripsQueryService
{
    Task<ApiResponse<UiPagedResult<UiTripDetailsDto>>> GetOperationManagerTripsAsync(UiFilterTripsDto filter, string bearerToken, CancellationToken cancellationToken = default);
}