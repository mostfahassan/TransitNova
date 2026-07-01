using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Governments.Command
{
    public class AdminGovernmentsCommand(IHttpHandler httpHandler, HttpClient httpClient) :IAdminGovernmentCommand
    {
        public async Task<ApiResponse<UiGovernmentDto>> CreateGovernmentAsync(UiCreateGovernmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateGovernmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.CreateGovernmentUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiGovernmentDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteGovernmentAsync(int governmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.DeleteGovernmentUrl, ("governmentId", governmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateGovernmentAsync(int governmentId, UiUpdateGovernmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateGovernmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.UpdateGovernmentUrl, ("governmentId", governmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
