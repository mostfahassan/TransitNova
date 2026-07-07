namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface ISubscribeToBundleCommandService
{
    Task<ApiResponse> SubscribeToBundleAsync(Guid bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

