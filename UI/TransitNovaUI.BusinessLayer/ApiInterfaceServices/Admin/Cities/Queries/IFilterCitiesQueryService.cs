namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;

public interface IFilterCitiesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/cities";

    Task<ApiResponse<UiPagedResult<UiCityDto>>> FilterCitiesAsync(UiCityFilterDto filter, CancellationToken cancellationToken = default);
}

