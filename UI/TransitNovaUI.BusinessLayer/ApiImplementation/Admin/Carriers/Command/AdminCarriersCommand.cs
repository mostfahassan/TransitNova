using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Segregations.Commands;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Carriers.Command
{
    public class AdminCarriersCommand(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminCarriersCommand
    {
        public Task<ApiResponse> DeleteCarrierAsync(Guid id, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            object? content = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminCarriers.DeleteCarrierUrl, ("id", id)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}


