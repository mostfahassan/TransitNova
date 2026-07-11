namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Bundles.Queries;

public interface IGetUserBundlesQueryService
{
    Task<ApiResponse<List<UiRetrieveBundleDto>>> GetUserBundlesAsync(string bearerToken, CancellationToken cancellationToken = default);
}
