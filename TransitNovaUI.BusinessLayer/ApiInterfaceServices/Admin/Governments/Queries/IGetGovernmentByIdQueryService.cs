namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;

public interface IGetGovernmentByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/governments/{governmentId:int}";

    Task<ApiResponse<UiGovernmentDto?>> GetGovernmentByIdAsync(int governmentId, CancellationToken cancellationToken = default);
}

