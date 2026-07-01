using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Commands;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Bundles.Command
{
    public class AdminBundlesCommand(IHttpHandler httpHandler, HttpClient httpClient) : IAdminBundlesCommand
    {
        public async Task<ApiResponse> CreateBundleAsync(UiCreateBundleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken)
        {
            var content = UiCreateBundleDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.CreateBundleUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            object? content = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.DeleteBundleUrl, ("bundleId", bundleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateBundleAsync(UiUpdateBundleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateBundleDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.UpdateBundleUrl, ("bundleId", model.BundleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
