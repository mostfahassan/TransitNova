using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Profile.Query
{
    public class OperationManagerProfileQuery(IHttpHandler httpHandler, HttpClient httpClient) 
        : IOperationManagerProfileQuery
    {
        public async Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerProfile.GetHandledCarriersUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerProfile.GetHandledShipmentsUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiRetrieveShipmentSummaryDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerProfile.GetOperationManagerByIdUrl, ("operationManagerId", operationManagerId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiOperationManagerProfileDto>(httpResponse, cancellationToken);
        }

    }
}
