namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Bundles.Queries;

public interface IGetUserBundleByIdQueryService
{
    Task<ApiResponse<UiRetrieveBundleDto?>> GetUserBundleByIdAsync(Guid bundleId, string bearerToken, CancellationToken cancellationToken = default);
}
