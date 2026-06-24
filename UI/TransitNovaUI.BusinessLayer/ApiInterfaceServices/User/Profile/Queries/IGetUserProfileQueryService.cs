namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;

public interface IGetUserProfileQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/users/profile";

    Task<ApiResponse<UiUserProfileDto>> GetUserProfileAsync(CancellationToken cancellationToken = default);
}
