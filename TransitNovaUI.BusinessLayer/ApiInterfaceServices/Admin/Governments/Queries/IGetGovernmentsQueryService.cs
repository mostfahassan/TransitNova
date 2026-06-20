namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;

public interface IGetGovernmentsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/governments";

    Task<ApiResponse<List<UiGovernmentDto>>> GetGovernmentsAsync(CancellationToken cancellationToken = default);
}

