namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Queries;

public interface IGetCarrierTripByIdQueryService
{
    Task<ApiResponse<UiCarrierTripDto>> GetCarrierTripByIdAsync(Guid carrierId, Guid tripId, string bearerToken, CancellationToken cancellationToken = default);
}
