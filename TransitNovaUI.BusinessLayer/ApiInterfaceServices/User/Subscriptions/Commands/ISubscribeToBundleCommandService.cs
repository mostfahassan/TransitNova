namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface ISubscribeToBundleCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/Subscription/bundles/{bundleId:int}/subscription";

    Task<ApiResponse> SubscribeToBundleAsync(int bundleId, CancellationToken cancellationToken = default);
}

