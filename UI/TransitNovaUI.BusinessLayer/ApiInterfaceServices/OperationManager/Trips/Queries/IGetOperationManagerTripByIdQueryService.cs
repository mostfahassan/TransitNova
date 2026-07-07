namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Queries;

public interface IGetOperationManagerTripByIdQueryService
{
    Task<ApiResponse<UiTripDetailsDto>> GetOperationManagerTripByIdAsync(Guid tripId, string bearerToken, CancellationToken cancellationToken = default);
}