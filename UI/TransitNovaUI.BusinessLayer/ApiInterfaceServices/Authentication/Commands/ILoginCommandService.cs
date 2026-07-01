namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface ILoginCommandService
{
    Task<ApiResponse<UiAuthResponseDto>> LoginAsync(UiLoginDto model, CancellationToken cancellationToken = default);
}

