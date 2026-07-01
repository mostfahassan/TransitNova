namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Queries;

public interface IGetCarrierTripsQueryService
{
    Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

