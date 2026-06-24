namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface ISignOutCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/auth/signout";

    Task<ApiResponse> SignOutAsync(CancellationToken cancellationToken = default);
}

