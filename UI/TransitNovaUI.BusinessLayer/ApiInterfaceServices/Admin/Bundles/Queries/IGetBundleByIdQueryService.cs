namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;

public interface IGetBundleByIdQueryService
{
    Task<ApiResponse<UiRetrieveBundleDto?>> GetBundleByIdAsync(int bundleId, string bearerToken, CancellationToken cancellationToken = default);
}

