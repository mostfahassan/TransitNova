using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Commands;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Bundles.Command
{
    public class AdminBundlesCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminBundlesCommand
    {
        public Task<ApiResponse> CreateBundleAsync(UiCreateBundleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken)
        {
            var content = UiCreateBundleDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.CreateBundleUrl));

            return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteBundleAsync(int bundleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            object? content = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.DeleteBundleUrl, ("bundleId", bundleId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> UpdateBundleAsync(UiUpdateBundleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateBundleDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.UpdateBundleUrl, ("bundleId", model.BundleId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
