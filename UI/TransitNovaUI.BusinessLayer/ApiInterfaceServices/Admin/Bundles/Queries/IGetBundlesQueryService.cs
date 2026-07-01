namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;

public interface IGetBundlesQueryService
{
    Task<ApiResponse<List<UiRetrieveBundleDto>>> GetBundlesAsync(string bearerToken, CancellationToken cancellationToken = default);
}

