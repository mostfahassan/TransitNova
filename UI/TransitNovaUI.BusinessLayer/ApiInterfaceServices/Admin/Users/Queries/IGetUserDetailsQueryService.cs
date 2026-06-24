namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Queries;

public interface IGetUserDetailsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/users/{userId:guid}";

    Task<ApiResponse<UiAdminUserDetailsDto>> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
}

