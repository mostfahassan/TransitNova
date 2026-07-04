namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetPublicCityByIdQueryService
{
    Task<ApiResponse<UiCityDto?>> GetPublicCityByIdAsync(int cityId, string? bearerToken = null, CancellationToken cancellationToken = default);
}

