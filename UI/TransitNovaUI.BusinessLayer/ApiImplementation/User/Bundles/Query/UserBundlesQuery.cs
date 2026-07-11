using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Bundles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Bundles.Query;

public sealed class UserBundlesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserBundlesQuery
{
    public Task<ApiResponse<UiRetrieveBundleDto?>> GetUserBundleByIdAsync(Guid bundleId, string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserBundles.GetUserBundleByIdUrl, ("bundleId", bundleId)));

        return SendQueryRequestAsync<UiRetrieveBundleDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }

    public Task<ApiResponse<List<UiRetrieveBundleDto>>> GetUserBundlesAsync(string bearerToken, CancellationToken cancellationToken = default)
    {
        var url = httpHandler.UrlBuilder(ApiRoutes.UserBundles.GetUserBundlesUrl);

        return SendQueryRequestAsync<List<UiRetrieveBundleDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
    }
}
