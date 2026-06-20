namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCountriesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/Country";

    Task<ApiResponse<IEnumerable<UiCountryDto>>> GetCountriesAsync(CancellationToken cancellationToken = default);
}

