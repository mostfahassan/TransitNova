using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Profile.Command
{
    public class CarrierProfileCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), ICarrierProfileCommand
    {
        public Task<ApiResponse> AddCarrierAdditionalInfoAsync(UiAdditionalInfoDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiAdditionalInfoDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierProfile.AddCarrierAdditionalInfoUrl));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<UiCarrierProfileDto>> UpdateCarrierProfileAsync(UiUpdateCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierProfile.UpdateCarrierProfileUrl));

            return SendRequestAsync<UiCarrierProfileDto>(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
