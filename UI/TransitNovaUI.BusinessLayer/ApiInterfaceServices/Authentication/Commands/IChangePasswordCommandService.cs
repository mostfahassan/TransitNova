namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IChangePasswordCommandService
{
    Task<ApiResponse> ChangePasswordAsync(UiChangePasswordDto model,string bearerToken, CancellationToken cancellationToken = default);
}

