namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Queries;

public interface IGetCarrierTripsQueryService
{
    Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}
