namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;

public interface IGetBundlesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/bundles";

    Task<ApiResponse<List<UiRetrieveBundleDto>>> GetBundlesAsync(CancellationToken cancellationToken = default);
}

