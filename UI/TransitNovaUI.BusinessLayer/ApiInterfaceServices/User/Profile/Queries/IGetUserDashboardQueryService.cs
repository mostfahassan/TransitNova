namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;

public interface IGetUserDashboardQueryService
{
    Task<ApiResponse<UiProfileDashboardDto>> GetUserDashboardAsync(string bearerToken, CancellationToken cancellationToken = default);
}

