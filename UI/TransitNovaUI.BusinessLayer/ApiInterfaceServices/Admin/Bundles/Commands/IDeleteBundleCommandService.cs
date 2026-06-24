namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

public interface IDeleteBundleCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admin/bundles/{bundleId:int}";

    Task<ApiResponse> DeleteBundleAsync(int bundleId, CancellationToken cancellationToken = default);
}

