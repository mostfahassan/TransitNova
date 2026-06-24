namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

public interface IGetBundleSubscribersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/subscriptions/bundles/{bundleId:int}/subscribers";

    Task<ApiResponse<List<UiUserProfileDto>>> GetBundleSubscribersAsync(int bundleId, CancellationToken cancellationToken = default);
}

