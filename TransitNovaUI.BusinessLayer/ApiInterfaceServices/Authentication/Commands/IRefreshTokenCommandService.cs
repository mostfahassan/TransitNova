namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IRefreshTokenCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/refreshToken";

    Task<ApiResponse<UiAuthResponseDto>> RefreshTokenAsync(UiRefreshTokenDto request, CancellationToken cancellationToken = default);
}

