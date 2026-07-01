using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Governments.Query
{
    public class AdminGovernmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) :IAdminGovernmentQuery
    {
        public async Task<ApiResponse<UiGovernmentDto?>> GetGovernmentByIdAsync(int governmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.GetGovernmentByIdUrl, ("governmentId", governmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiGovernmentDto?>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<List<UiGovernmentDto>>> GetGovernmentsAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.GetGovernmentsUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiGovernmentDto>>(httpResponse, cancellationToken);
        }

    }
}
