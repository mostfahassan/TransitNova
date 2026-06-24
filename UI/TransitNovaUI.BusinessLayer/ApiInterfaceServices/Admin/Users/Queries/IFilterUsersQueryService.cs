namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Queries;

public interface IFilterUsersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/users";

    Task<ApiResponse<UiPagedResult<UiAdminUserDetailsDto>>> FilterUsersAsync(UiUserFiltrationDto filter, CancellationToken cancellationToken = default);
}

