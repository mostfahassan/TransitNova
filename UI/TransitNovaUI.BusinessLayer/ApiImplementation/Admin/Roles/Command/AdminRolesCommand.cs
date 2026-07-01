using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Roles.Command
{
    public class AdminRolesCommand(IHttpHandler httpHandler, HttpClient httpClient): IAdminRolesCommand

    {
        public async Task<ApiResponse> CreateRoleAsync(UiRoleNameDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRoleNameDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.CreateRoleUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> DeleteRoleAsync(Guid roleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.DeleteRoleUrl, ("roleId", roleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateRoleAsync(Guid roleId, UiRoleNameDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiRoleNameDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.UpdateRoleUrl, ("roleId", roleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put,url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> UpdateRoleMembersAsync(Guid roleId, UiUpdateRoleMembersDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var content = UiUpdateRoleMembersDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Roles.UpdateRoleMembersUrl, ("roleId", roleId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
