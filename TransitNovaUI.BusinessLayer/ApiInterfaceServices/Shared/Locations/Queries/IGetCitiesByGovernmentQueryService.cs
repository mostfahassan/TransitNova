namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCitiesByGovernmentQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/City/{governmentId}/cities";

    Task<ApiResponse<IEnumerable<UiCityDto>>> GetCitiesByGovernmentAsync(int governmentId, CancellationToken cancellationToken = default);
}

