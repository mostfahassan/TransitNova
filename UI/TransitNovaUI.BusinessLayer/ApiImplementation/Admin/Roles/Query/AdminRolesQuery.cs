using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Roles.Query
{
    public class AdminRolesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminRolesQuery
    {
        public Task<ApiResponse<UiRoleMembersDto>> GetRoleByIdAsync(Guid roleId, string bearerToken, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.GetRoleByIdUrl, ("roleId", roleId), ("pageNumber", pageNumber), ("pageSize", pageSize)));

            return SendQueryRequestAsync<UiRoleMembersDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiRoleMembersDto>> GetRoleMembersAsync(Guid roleId, string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.GetRoleMembersUrl, ("roleId", roleId)));

            return SendQueryRequestAsync<UiRoleMembersDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<List<UiRoleSummaryDto>>> GetRolesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.GetRolesUrl));

            return SendQueryRequestAsync<List<UiRoleSummaryDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
