namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface IUnsubscribeFromBundleCommandService
{
    Task<ApiResponse> UnsubscribeFromBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

