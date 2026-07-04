using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Dashboard.Segregation;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Dashboard.Query
{
    public class CarrierDashboardQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ICarrierDashboardQuery
    {
        public Task<ApiResponse<UiCarrierDashboardDto>> GetCarrierDashboardAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierDashboard.GetCarrierDashboardUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<UiCarrierDashboardDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}


