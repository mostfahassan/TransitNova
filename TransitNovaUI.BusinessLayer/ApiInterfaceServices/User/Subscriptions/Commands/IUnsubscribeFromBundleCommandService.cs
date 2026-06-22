namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface IUnsubscribeFromBundleCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/subscriptions/bundles/{bundleId:guid}/subscription";

    Task<ApiResponse> UnsubscribeFromBundleAsync(int bundleId, CancellationToken cancellationToken = default);
}
