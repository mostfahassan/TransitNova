namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface ILoginCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/auth/login";

    Task<ApiResponse<UiAuthResponseDto>> LoginAsync(UiLoginDto request, CancellationToken cancellationToken = default);
}

