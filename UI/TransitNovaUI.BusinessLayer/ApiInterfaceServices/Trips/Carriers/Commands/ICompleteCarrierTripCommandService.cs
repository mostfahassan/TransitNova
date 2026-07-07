namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Commands
{
    public interface ICompleteCarrierTripCommandService
    {
        Task<ApiResponse> CompleteCarrierTripAsync(Guid carrierId, Guid tripId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
    }
}