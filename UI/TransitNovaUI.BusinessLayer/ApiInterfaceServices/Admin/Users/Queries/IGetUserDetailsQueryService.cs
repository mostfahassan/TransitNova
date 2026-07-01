namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Queries;

public interface IGetUserDetailsQueryService
{
    Task<ApiResponse<UiAdminUserDetailsDto>> GetUserDetailsAsync(Guid userId, string bearerToken, CancellationToken cancellationToken = default);
}

