namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetPublicCitiesQueryService
{
    Task<ApiResponse<UiPagedResult<UiCityDto>>> GetPublicCitiesAsync(UiCityFilterDto filter, string? bearerToken = null, CancellationToken cancellationToken = default);
}

