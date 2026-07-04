using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Commands;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Cities.Command
{
    public class AdminCitiesCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminCityCommand
    {
        public Task<ApiResponse<UiCityDto>> CreateCityAsync(UiCreateCityDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateCityDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.CreateCityUrl));

            return SendRequestAsync<UiCityDto>(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteCityAsync(int cityId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.DeleteCityUrl, ("cityId", cityId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken,null,idempotentKey);
        }

        public Task<ApiResponse> UpdateCityAsync(int cityId, UiUpdateCityDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateCityDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.UpdateCityUrl, ("cityId", cityId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
