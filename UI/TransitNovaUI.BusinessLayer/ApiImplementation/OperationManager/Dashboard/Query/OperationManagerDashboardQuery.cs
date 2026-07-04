using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Segregation;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Dashboard.Query
{
    public class OperationManagerDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IOperationManagerDashboardQuery
    {
        public Task<ApiResponse<UiOperationManagerDashboardDto>> GetOperationManagerDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerDashboard.GetOperationManagerDashboardUrl));

            return SendQueryRequestAsync<UiOperationManagerDashboardDto>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}


