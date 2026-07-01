namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;

public interface IFilterCitiesQueryService
{
    Task<ApiResponse<UiPagedResult<UiCityDto>>> FilterCitiesAsync(UiCityFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

