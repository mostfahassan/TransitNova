namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface IDeleteBundleCommandService
{
    Task<ApiResponse> DeleteBundleAsync(Guid bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

