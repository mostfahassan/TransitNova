using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Dashboard.Queries;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Dashboard.Query
{
    public class AdminDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : IGetAdminDashboardQueryService
    {
        public async Task<ApiResponse<UiAdminDashboardDto>> GetAdminDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
        
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminDashboard.GetAdminDashboardUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiAdminDashboardDto>(httpResponse, cancellationToken);
        }
    }
}
