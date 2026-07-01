namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Queries;

public interface IGetCarrierTripByIdQueryService
{
    Task<ApiResponse<UiCarrierTripDto>> GetCarrierTripByIdAsync(Guid carrierId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default);
}

