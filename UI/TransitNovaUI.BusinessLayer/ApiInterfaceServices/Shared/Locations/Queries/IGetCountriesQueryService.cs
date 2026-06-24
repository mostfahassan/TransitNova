namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCountriesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/countries";

    Task<ApiResponse<IEnumerable<UiCountryDto>>> GetCountriesAsync(CancellationToken cancellationToken = default);
}
