using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Dashboard.Queries;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Dashboard.Query
{
    public class OperationManagerDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : IGetOperationManagerDashboardQueryService
    {
        public async Task<ApiResponse<UiOperationManagerDashboardDto>> GetOperationManagerDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerDashboard.GetOperationManagerDashboardUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiOperationManagerDashboardDto>(httpResponse, cancellationToken);
        }

    }
}
