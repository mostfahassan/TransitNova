using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Authentication.Command
{
    public class AuthenticationCommand(IHttpHandler httpHandler, HttpClient httpClient): ApiServiceBase(httpHandler, httpClient), IAuthenticationCommand

    {
        public Task<ApiResponse> ChangePasswordAsync(UiChangePasswordDto model, string bearerToken, CancellationToken cancellationToken = default)
        {
            var content = UiChangePasswordDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.ChangePasswordUrl));

            return SendRequestAsync(HttpMethod.Put, url, bearerToken, cancellationToken, content);
        }

        public Task<ApiResponse<UiAuthResponseDto>> LoginAsync(UiLoginDto model, CancellationToken cancellationToken = default)
        {
            var content = UiLoginDto.ToDto(model);
    
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.LoginUrl));

            return SendRequestAsync<UiAuthResponseDto>(HttpMethod.Post, url, null, cancellationToken, content);
        }

        public Task<ApiResponse<UiAuthResponseDto>> RefreshTokenAsync(UiRefreshTokenDto model, CancellationToken cancellationToken = default)
        {
            var content = UiRefreshTokenDto.ToDto(model);
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.RefreshTokenUrl));

            return SendRequestAsync<UiAuthResponseDto>(HttpMethod.Post, url, null, cancellationToken, content);
        }

        public Task<ApiResponse<UiAuthResponseDto>> RegisterAsync(UiRegisterDto model, CancellationToken cancellationToken = default)
        {
            var content = UiRegisterDto.ToDto(model);
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.RegisterUrl));

            return SendRequestAsync<UiAuthResponseDto>(HttpMethod.Post, url, null, cancellationToken, content,string.Empty);
        }

        public Task<ApiResponse> RevokeRefreshTokenAsync(Guid userId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.RevokeRefreshTokenUrl, ("userId", userId)));

            return SendRequestAsync(HttpMethod.Delete, url, bearerToken, cancellationToken,null, idempotentKey);
        }

        public Task<ApiResponse> SignOutAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Authentication.SignOutUrl));

            return SendRequestAsync(HttpMethod.Post, url, bearerToken, cancellationToken);
        }

    }
}
