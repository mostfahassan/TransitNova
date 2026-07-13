namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

public interface IGetBundleSubscribersQueryService
{
    Task<ApiResponse<List<UiBundleSubscriptionDetailsDto>>> GetSubscribersAsync(string bearerToken, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<UiBundleSubscriptionDetailsDto>>> GetBundleSubscribersAsync(Guid bundleId, string bearerToken, CancellationToken cancellationToken = default);
}