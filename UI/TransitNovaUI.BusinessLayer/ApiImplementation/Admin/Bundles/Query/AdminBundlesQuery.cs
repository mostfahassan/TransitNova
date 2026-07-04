using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Bundles.Query
{
    public class AdminBundlesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminBundlesQuery
    {
        public Task<ApiResponse<UiRetrieveBundleDto?>> GetBundleByIdAsync(int bundleId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.GetBundleByIdUrl, ("bundleId", bundleId)));

            return SendQueryRequestAsync<UiRetrieveBundleDto?>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<List<UiRetrieveBundleDto>>> GetBundlesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Bundles.GetBundlesUrl));

            return SendQueryRequestAsync<List<UiRetrieveBundleDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
