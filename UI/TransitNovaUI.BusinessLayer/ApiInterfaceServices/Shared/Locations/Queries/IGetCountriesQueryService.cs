namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCountriesQueryService
{
    Task<ApiResponse<IEnumerable<UiCountryDto>>> GetCountriesAsync(string bearerToken, CancellationToken cancellationToken = default);
}

