using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Bundles.Query
{
    public class AdminBundlesQuery(IHttpHandler httpHandler, HttpClient httpClient) : IAdminBundlesQuery
    {
        public async Task<ApiResponse<UiRetrieveBundleDto?>> GetBundleByIdAsync(int bundleId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.GetBundleByIdUrl, ("bundleId", bundleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRetrieveBundleDto?>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<List<UiRetrieveBundleDto>>> GetBundlesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.GetBundlesUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiRetrieveBundleDto>>(httpResponse, cancellationToken);
        }

    }
}
