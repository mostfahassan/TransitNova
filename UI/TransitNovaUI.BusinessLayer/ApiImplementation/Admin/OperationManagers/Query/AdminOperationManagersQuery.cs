using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.OperationManagers.Segregation;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.OperationManagers.Query
{
    public class AdminOperationManagersQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminOperationManagerQuery
    {
        public Task<ApiResponse<List<UiOperationManagerProfileDto>>> GetActiveOperationManagersAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetActiveOperationManagersUrl));

            return SendQueryRequestAsync<List<UiOperationManagerProfileDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiPagedResult<UiCarrierSummaryDetailsDto>>> GetHandledCarriersAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
      
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetHandledCarriersUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiCarrierSummaryDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiPagedResult<UiRetrieveShipmentSummaryDto>>> GetHandledShipmentsAsync(Guid operationManagerId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetHandledShipmentsUrl, ("operationManagerId", operationManagerId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiRetrieveShipmentSummaryDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiOperationManagerProfileDto>> GetOperationManagerByIdAsync(Guid operationManagerId, string bearerToken, CancellationToken cancellationToken = default)
        {
       
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetOperationManagerByIdUrl, ("operationManagerId", operationManagerId)));

            return SendQueryRequestAsync<UiOperationManagerProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<IEnumerable<UiOperationManagerProfileDto>>> GetOperationManagersAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
         
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminOperationManagers.GetOperationManagersUrl));

            return SendQueryRequestAsync<IEnumerable<UiOperationManagerProfileDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
