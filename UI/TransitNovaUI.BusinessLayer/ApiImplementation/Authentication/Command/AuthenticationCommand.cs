using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Authentication.Command
{
    public class AuthenticationCommand(IHttpHandler httpHandler, HttpClient httpClient): IAuthenticationCommand

    {
        public async Task<ApiResponse> ChangePasswordAsync(UiChangePasswordDto model, string bearerToken, CancellationToken cancellationToken = default)
        {
            var content = UiChangePasswordDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.ChangePasswordUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Put, url, bearerToken, cancellationToken, content);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiAuthResponseDto>> LoginAsync(UiLoginDto model, CancellationToken cancellationToken = default)
        {
            var content = UiLoginDto.ToDto(model);
    
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.LoginUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post,  url, null, cancellationToken, content);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiAuthResponseDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiAuthResponseDto>> RefreshTokenAsync(UiRefreshTokenDto model, CancellationToken cancellationToken = default)
        {
            var content = UiRefreshTokenDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.RefreshTokenUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post, url, null, cancellationToken, content);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiAuthResponseDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiAuthResponseDto>> RegisterAsync(UiRegisterDto model, CancellationToken cancellationToken = default)
        {
            var content = UiRegisterDto.ToDto(model);
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.RegisterUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post,  url, null, cancellationToken,content);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<UiAuthResponseDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> RevokeRefreshTokenAsync(Guid userId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.RevokeRefreshTokenUrl, ("userId", userId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Delete,  url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse> SignOutAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.SignOutUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Post,url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

    }
}
