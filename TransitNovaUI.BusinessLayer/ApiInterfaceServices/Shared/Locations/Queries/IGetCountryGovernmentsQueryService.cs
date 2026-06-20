namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetCountryGovernmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/Country/{governmentId:int}/governments";

    Task<ApiResponse<IEnumerable<UiGovernmentDto>>> GetCountryGovernmentsAsync(int governmentId, CancellationToken cancellationToken = default);
}

