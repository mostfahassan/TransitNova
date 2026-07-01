namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

public interface IRegisterCommandService
{
    Task<ApiResponse<UiAuthResponseDto>> RegisterAsync(UiRegisterDto model, CancellationToken cancellationToken = default);
}

