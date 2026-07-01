namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

public interface IUpdateCityCommandService
{
    Task<ApiResponse> UpdateCityAsync(int cityId, UiUpdateCityDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

