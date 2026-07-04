using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Governments.Command
{
    public class AdminGovernmentsCommand(IHttpHandler httpHandler, HttpClient httpClient) :ApiServiceBase(httpHandler, httpClient), IAdminGovernmentCommand
    {
        public Task<ApiResponse<UiGovernmentDto>> CreateGovernmentAsync(UiCreateGovernmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiCreateGovernmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.CreateGovernmentUrl));

            return SendRequestAsync<UiGovernmentDto>(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteGovernmentAsync(int governmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.DeleteGovernmentUrl, ("governmentId", governmentId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);
        }

        public Task<ApiResponse> UpdateGovernmentAsync(int governmentId, UiUpdateGovernmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateGovernmentDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Governments.UpdateGovernmentUrl, ("governmentId", governmentId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
