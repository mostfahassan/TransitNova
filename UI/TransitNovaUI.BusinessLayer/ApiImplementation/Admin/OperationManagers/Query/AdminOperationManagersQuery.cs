using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Segregation;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.OperationManagers.Query
{
    public class AdminOperationManagersQuery(IHttpHandler httpHandler, HttpClient httpClient) : IAdminOperationManagerQuery
    {
        public async Task<ApiResponse<List<UiOperationManagerProfileDto>>> GetActiveOperationManagersAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetActiveOperationManagersUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiOperationManagerProfileDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
      
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetHandledCarriersUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetHandledShipmentsUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiRetrieveShipmentSummaryDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, string bearerToken, CancellationToken cancellationToken = default)
        {
       
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetOperationManagerByIdUrl, ("operationManagerId", operationManagerId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiOperationManagerProfileDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UiOperationManagerProfileDto>>> GetOperationManagersAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
         
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetOperationManagersUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IEnumerable<UiOperationManagerProfileDto>>(httpResponse, cancellationToken);
        }

    }
}
