namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Commands;

public interface IStartPickupTripCommandService
{
    Task<ApiResponse> StartPickupTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

