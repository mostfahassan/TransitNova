namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IChangePasswordCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/auth/change-password";

    Task<ApiResponse> ChangePasswordAsync(UiChangePasswordDto request, CancellationToken cancellationToken = default);
}

