namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface IUpdateBundleCommandService
{
    Task<ApiResponse> UpdateBundleAsync(UiUpdateBundleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

