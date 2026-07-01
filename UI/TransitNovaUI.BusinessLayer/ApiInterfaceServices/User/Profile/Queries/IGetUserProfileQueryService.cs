namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;

public interface IGetUserProfileQueryService
{
    Task<ApiResponse<UiUserProfileDto>> GetUserProfileAsync(string bearerToken, CancellationToken cancellationToken = default);
}

