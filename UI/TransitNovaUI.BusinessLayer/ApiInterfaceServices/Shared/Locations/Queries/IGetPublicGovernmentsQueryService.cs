namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetPublicGovernmentsQueryService
{
    Task<ApiResponse<List<UiGovernmentDto>>> GetPublicGovernmentsAsync(string? bearerToken = null, CancellationToken cancellationToken = default);
}

