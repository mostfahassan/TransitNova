namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCountryGovernmentsQueryService
{
    Task<ApiResponse<IEnumerable<UiGovernmentDto>>> GetCountryGovernmentsAsync(int countryId, string bearerToken, CancellationToken cancellationToken = default);
}

