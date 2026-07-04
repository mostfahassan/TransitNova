using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Governments.Query
{
    public class AdminGovernmentsQuery(IHttpHandler httpHandler, HttpClient httpClient) :ApiServiceBase(httpHandler, httpClient), IAdminGovernmentQuery
    {
        public Task<ApiResponse<UiGovernmentDto?>> GetGovernmentByIdAsync(int governmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.GetGovernmentByIdUrl, ("governmentId", governmentId)));

            return SendQueryRequestAsync<UiGovernmentDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<List<UiGovernmentDto>>> GetGovernmentsAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.GetGovernmentsUrl));

            return SendQueryRequestAsync<List<UiGovernmentDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
