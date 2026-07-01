using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Commands;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Carriers.Command
{
    public class AdminCarriersCommand(IHttpHandler httpHandler, HttpClient httpClient) : IDeleteCarrierCommandService
    {
        public async Task<ApiResponse> DeleteCarrierAsync(Guid id, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            object? content = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminCarriers.DeleteCarrierUrl, ("id", id)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
