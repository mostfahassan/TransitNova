using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Profile.Query
{
    public class CarrierProfileQuery(IHttpHandler httpHandler, HttpClient httpClient): ApiServiceBase(httpHandler, httpClient), ICarrierProfileQuery
    {
        public Task<ApiResponse<UiCarrierProfileDto>> GetCarrierProfileAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierProfile.GetCarrierProfileUrl, ("carrierId", carrierId)));

            return SendQueryRequestAsync<UiCarrierProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
