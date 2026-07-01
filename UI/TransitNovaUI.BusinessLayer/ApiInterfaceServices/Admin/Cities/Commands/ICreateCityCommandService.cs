namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

public interface ICreateCityCommandService
{
    Task<ApiResponse<UiCityDto>> CreateCityAsync(UiCreateCityDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

