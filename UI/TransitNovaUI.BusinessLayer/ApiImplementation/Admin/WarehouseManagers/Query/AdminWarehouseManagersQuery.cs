using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.WarehouseManagers.Segregations.Query;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.WarehouseManagers.Query
{
    public class AdminWarehouseManagersQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminWarehouseManagersQuery
    {
        public Task<ApiResponse<UiWarehouseManagerDetailsDto>> GetWarehouseManagerByIdAsync(Guid id, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminWarehouseManagers.GetWarehouseManagerByIdUrl, ("id", id)));

            return SendQueryRequestAsync<UiWarehouseManagerDetailsDto>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<UiPagedResult<UiWarehouseManagerListDto>>> GetWarehouseManagersAsync(UiWarehouseManagerFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminWarehouseManagers.GetWarehouseManagersUrl, ("FullName", filter.FullName), ("Email", filter.Email), ("WarehouseId", filter.WarehouseId), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiWarehouseManagerListDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }
    }
}
