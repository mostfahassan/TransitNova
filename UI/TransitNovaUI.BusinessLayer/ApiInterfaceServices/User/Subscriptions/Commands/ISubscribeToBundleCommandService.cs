namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface ISubscribeToBundleCommandService
{
    Task<ApiResponse> SubscribeToBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

