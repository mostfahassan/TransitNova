namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface ICreateBundleCommandService
{
    Task<ApiResponse> CreateBundleAsync(UiCreateBundleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken);
}

