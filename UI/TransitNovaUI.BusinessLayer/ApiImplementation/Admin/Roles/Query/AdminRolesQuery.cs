using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Roles.Query
{
    public class AdminRolesQuery(IHttpHandler httpHandler, HttpClient httpClient) : IAdminRolesQuery
    {
        public async Task<ApiResponse<UiRoleSummaryDto>> GetRoleByIdAsync(Guid roleId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.GetRoleByIdUrl, ("roleId", roleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRoleSummaryDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiRoleMembersDto>> GetRoleMembersAsync(Guid roleId, string bearerToken, CancellationToken cancellationToken = default)
        {
           
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.GetRoleMembersUrl, ("roleId", roleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiRoleMembersDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<List<UiRoleSummaryDto>>> GetRolesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.GetRolesUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<List<UiRoleSummaryDto>>(httpResponse, cancellationToken);
        }

    }
}
