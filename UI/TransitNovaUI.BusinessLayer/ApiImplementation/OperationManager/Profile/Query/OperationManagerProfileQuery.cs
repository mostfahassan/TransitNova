using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Profile.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.OperationManager.Profile.Query
{
    public class OperationManagerProfileQuery(IHttpHandler httpHandler, HttpClient httpClient) 
        : ApiServiceBase(httpHandler, httpClient), IOperationManagerProfileQuery
    {
        public Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerProfile.GetHandledCarriersUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerProfile.GetHandledShipmentsUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiRetrieveShipmentSummaryDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.OperationManagerProfile.GetOperationManagerByIdUrl, ("operationManagerId", operationManagerId)));

            return SendQueryRequestAsync<UiOperationManagerProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
