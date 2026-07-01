namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Queries;

public interface IFilterUsersQueryService
{
    Task<ApiResponse<UiPagedResult<UiAdminUserDetailsDto>>> FilterUsersAsync(UiUserFiltrationDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

