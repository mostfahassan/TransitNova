namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Commands;

public interface IStartDeliveryTripCommandService
{
    Task<ApiResponse> StartDeliveryTripAsync(Guid carrierId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

