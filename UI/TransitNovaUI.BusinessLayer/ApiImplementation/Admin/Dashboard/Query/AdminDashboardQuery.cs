using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Segregation;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Dashboard.Query
{
    public class AdminDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminDashboardQuery
    {
        public Task<ApiResponse<UiAdminDashboardDto>> GetAdminDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
        
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminDashboard.GetAdminDashboardUrl));

            return SendQueryRequestAsync<UiAdminDashboardDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}


