namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;

public interface IGetBundleByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/bundles/{bundleId:int}";

    Task<ApiResponse<UiRetrieveBundleDto?>> GetBundleByIdAsync(int bundleId, CancellationToken cancellationToken = default);
}

