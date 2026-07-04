using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Segregation;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.User.Profile.Query
{
    public class UserProfileQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IUserProfileQuery
    {
        public Task<ApiResponse<UiProfileDashboardDto>> GetUserDashboardAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
        
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserProfile.GetUserDashboardUrl));

            return SendQueryRequestAsync<UiProfileDashboardDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiUserProfileDto>> GetUserProfileAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.UserProfile.GetUserProfileUrl));

            return SendQueryRequestAsync<UiUserProfileDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
