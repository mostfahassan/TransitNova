using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Queries;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Dashboard.Query
{
    public class CarrierDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : IGetCarrierDashboardQueryService
    {
        public async Task<ApiResponse<UiCarrierDashboardDto>> GetCarrierDashboardAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierDashboard.GetCarrierDashboardUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get,url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierDashboardDto>(httpResponse, cancellationToken);
        }

    }
}
