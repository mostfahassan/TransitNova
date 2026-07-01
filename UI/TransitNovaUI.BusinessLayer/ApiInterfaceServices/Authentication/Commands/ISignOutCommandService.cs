namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface ISignOutCommandService
{
    Task<ApiResponse> SignOutAsync(string bearerToken,CancellationToken cancellationToken = default);
}

