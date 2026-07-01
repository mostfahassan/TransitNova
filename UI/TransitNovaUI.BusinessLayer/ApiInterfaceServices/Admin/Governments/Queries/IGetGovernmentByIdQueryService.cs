namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;

public interface IGetGovernmentByIdQueryService
{
    Task<ApiResponse<UiGovernmentDto?>> GetGovernmentByIdAsync(int governmentId, string bearerToken, CancellationToken cancellationToken = default);
}

