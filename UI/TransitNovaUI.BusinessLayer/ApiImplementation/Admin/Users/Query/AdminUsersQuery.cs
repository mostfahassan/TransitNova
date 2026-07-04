
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Segregation;
using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Users.Query
{
    public class AdminUsersQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminUserQuery
    {
        public Task<ApiResponse<UiPagedResult<UiAdminUserDetailsDto>>> FilterUsersAsync(UiUserFiltrationDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminUsers.FilterUsersUrl, ("SearchTerm", filter.SearchTerm), ("Email", filter.Email), ("UserName", filter.UserName), ("PhoneNumber", filter.PhoneNumber), ("IsActive", filter.IsActive), ("CreatedFrom", filter.CreatedFrom), ("CreatedTo", filter.CreatedTo), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize)));

            return SendQueryRequestAsync<UiPagedResult<UiAdminUserDetailsDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiAdminUserDetailsDto>> GetUserDetailsAsync(Guid userId, string bearerToken, CancellationToken cancellationToken = default)
        {
      
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.AdminUsers.GetUserDetailsUrl, ("userId", userId)));

            return SendQueryRequestAsync<UiAdminUserDetailsDto>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
