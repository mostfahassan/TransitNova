namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

public interface IGetPublicGovernmentByIdQueryService
{
    Task<ApiResponse<UiGovernmentDto?>> GetPublicGovernmentByIdAsync(int governmentId, string? bearerToken = null, CancellationToken cancellationToken = default);
}

