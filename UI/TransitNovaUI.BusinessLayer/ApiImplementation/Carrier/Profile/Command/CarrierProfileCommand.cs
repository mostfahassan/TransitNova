using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Carrier.Profile.Command
{
    public class CarrierProfileCommand(IHttpHandler httpHandler, HttpClient httpClient) : ICarrierProfileCommand
    {
        public async Task<ApiResponse> AddCarrierAdditionalInfoAsync(UiAdditionalInfoDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiAdditionalInfoDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierProfile.AddCarrierAdditionalInfoUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Put,  url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiCarrierProfileDto>> UpdateCarrierProfileAsync(UiUpdateCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateCarrierDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.CarrierProfile.UpdateCarrierProfileUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Put,  url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiCarrierProfileDto>(httpResponse, cancellationToken);
        }

    }
}
