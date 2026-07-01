namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface IDeleteBundleCommandService
{
    Task<ApiResponse> DeleteBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

