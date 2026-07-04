namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Commands;

public interface IStartPickupTripCommandService
{
    Task<ApiResponse> StartPickupTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}
