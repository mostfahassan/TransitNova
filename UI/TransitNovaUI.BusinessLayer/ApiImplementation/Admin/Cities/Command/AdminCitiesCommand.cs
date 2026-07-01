using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Commands;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Cities.Command
{
    public class AdminCitiesCommand(IHttpHandler httpHandler, HttpClient httpClient) : IAdminCityCommmand
    {
        public async Task<ApiResponse<UiCityDto>> CreateCityAsync(UiCreateCityDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateCityDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.CreateCityUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiCityDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteCityAsync(int cityId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.DeleteCityUrl, ("cityId", cityId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateCityAsync(int cityId, UiUpdateCityDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateCityDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.UpdateCityUrl, ("cityId", cityId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
