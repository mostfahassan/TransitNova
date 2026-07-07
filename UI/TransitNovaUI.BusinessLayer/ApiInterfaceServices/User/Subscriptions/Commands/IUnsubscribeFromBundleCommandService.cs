namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface IUnsubscribeFromBundleCommandService
{
    Task<ApiResponse> UnsubscribeFromBundleAsync(Guid bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

