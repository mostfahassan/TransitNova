namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IRegisterCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/auth/register";

    Task<ApiResponse<UiAuthResponseDto>> RegisterAsync(UiRegisterDto request, CancellationToken cancellationToken = default);
}

