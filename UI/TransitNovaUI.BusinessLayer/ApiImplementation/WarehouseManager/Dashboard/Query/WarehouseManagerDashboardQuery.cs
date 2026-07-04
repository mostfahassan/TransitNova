using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Dashboard.Query
{
    public class WarehouseManagerDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IWarehouseManagerDashboardQuery
    {
        public Task<ApiResponse<UiWarehouseManagerDashboardDto>> GetWarehouseManagerDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
     
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerDashboard.GetDashboardUrl));

            return SendQueryRequestAsync<UiWarehouseManagerDashboardDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }
    }
}
