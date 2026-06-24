namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;

public interface IGetCityByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/cities/{cityId:int}";

    Task<ApiResponse<UiCityDto?>> GetCityByIdAsync(int cityId, CancellationToken cancellationToken = default);
}

