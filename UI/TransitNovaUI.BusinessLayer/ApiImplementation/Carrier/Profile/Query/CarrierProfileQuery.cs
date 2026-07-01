using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Profile.Query
{
    public class CarrierProfileQuery(IHttpHandler httpHandler, HttpClient httpClient): ICarrierProfileQuery
    {
        public async Task<ApiResponse<UiCarrierProfileDto>> GetCarrierProfileAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierProfile.GetCarrierProfileUrl, ("carrierId", carrierId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCarrierProfileDto>(httpResponse, cancellationToken);
        }

    }
}
