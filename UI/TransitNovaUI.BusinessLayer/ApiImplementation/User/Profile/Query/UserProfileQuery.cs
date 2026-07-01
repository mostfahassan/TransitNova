using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Segregation;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Profile.Query
{
    public class UserProfileQuery(IHttpHandler httpHandler, HttpClient httpClient) : IUserProfileQuery
    {
        public async Task<ApiResponse<UiProfileDashboardDto>> GetUserDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
        
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserProfile.GetUserDashboardUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiProfileDashboardDto>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiUserProfileDto>> GetUserProfileAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserProfile.GetUserProfileUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiUserProfileDto>(httpResponse, cancellationToken);
        }

    }
}
