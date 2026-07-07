namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;

public interface IGetBundleByIdQueryService
{
    Task<ApiResponse<UiRetrieveBundleDto?>> GetBundleByIdAsync(Guid bundleId, string bearerToken, CancellationToken cancellationToken = default);
}

