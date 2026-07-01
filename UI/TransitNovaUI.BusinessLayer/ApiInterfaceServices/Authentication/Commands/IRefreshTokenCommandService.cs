namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IRefreshTokenCommandService
{
    Task<ApiResponse<UiAuthResponseDto>> RefreshTokenAsync(UiRefreshTokenDto model, CancellationToken cancellationToken = default);
}

