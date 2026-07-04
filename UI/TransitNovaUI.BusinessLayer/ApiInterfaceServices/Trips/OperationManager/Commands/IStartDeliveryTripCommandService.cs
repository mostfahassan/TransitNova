namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Commands;

public interface IStartDeliveryTripCommandService
{
    Task<ApiResponse> StartDeliveryTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}
