namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface IUnsubscribeFromBundleCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/Subscription/bundles/{bundleId:int}/subscription";

    Task<ApiResponse> UnsubscribeFromBundleAsync(int bundleId, CancellationToken cancellationToken = default);
}

