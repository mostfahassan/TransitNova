namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;

public interface IGetCityByIdQueryService
{
    Task<ApiResponse<UiCityDto?>> GetCityByIdAsync(int cityId, string bearerToken, CancellationToken cancellationToken = default);
}

