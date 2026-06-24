namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface ICreateBundleCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admin/bundles";

    Task<ApiResponse> CreateBundleAsync(UiCreateBundleDto request, CancellationToken cancellationToken = default);
}

