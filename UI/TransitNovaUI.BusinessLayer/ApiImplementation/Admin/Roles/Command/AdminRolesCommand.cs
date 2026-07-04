using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Roles.Command
{
    public class AdminRolesCommand(IHttpHandler httpHandler, HttpClient httpClient): ApiServiceBase(httpHandler, httpClient), IAdminRolesCommand

    {
        public Task<ApiResponse> CreateRoleAsync(UiRoleNameDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRoleNameDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.CreateRoleUrl));

            return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> DeleteRoleAsync(Guid roleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.DeleteRoleUrl, ("roleId", roleId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken, null, idempotentKey);
        }

        public Task<ApiResponse> UpdateRoleAsync(Guid roleId, UiRoleNameDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRoleNameDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.UpdateRoleUrl, ("roleId", roleId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse> UpdateRoleMembersAsync(Guid roleId, UiUpdateRoleMembersDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateRoleMembersDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.UpdateRoleMembersUrl, ("roleId", roleId)));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);
        }

    }
}
