using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Commands;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.WarehouseManager.Dashboard.Command
{
    public class WarehouseManagerDashboardCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IWarehouseManagerDashboardCommand
    {
        public Task<ApiResponse> UpdateWarehouseManagerAsync(UiUpdateWarehouseManagerProfileDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateWarehouseManagerProfileDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.WarehouseManagerDashboard.UpdateWarehouseManagerUrl));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }
    }
}
