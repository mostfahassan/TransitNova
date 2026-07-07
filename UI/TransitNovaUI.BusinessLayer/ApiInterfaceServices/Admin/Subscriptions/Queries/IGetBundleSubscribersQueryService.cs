namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Subscriptions.Queries;

public interface IGetBundleSubscribersQueryService
{
    Task<ApiResponse<List<UiUserProfileDto>>> GetBundleSubscribersAsync(Guid bundleId, string bearerToken, CancellationToken cancellationToken = default);
}

