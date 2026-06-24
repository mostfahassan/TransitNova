namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface IUpdateBundleCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admin/bundles";

    Task<ApiResponse> UpdateBundleAsync(UiUpdateBundleDto request, CancellationToken cancellationToken = default);
}

