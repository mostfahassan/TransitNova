namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCitiesByGovernmentQueryService
{
    Task<ApiResponse<IEnumerable<UiCityDto>>> GetCitiesByGovernmentAsync(int governmentId, string bearerToken, CancellationToken cancellationToken = default);
}

