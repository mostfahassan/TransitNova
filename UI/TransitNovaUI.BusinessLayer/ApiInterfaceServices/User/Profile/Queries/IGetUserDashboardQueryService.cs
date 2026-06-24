namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Profile.Queries;

public interface IGetUserDashboardQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/user/dashboard";

    Task<ApiResponse<UiProfileDashboardDto>> GetUserDashboardAsync(CancellationToken cancellationToken = default);
}

