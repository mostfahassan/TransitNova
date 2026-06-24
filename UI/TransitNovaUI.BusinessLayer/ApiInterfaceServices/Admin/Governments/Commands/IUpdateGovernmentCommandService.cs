namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

public interface IUpdateGovernmentCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admin/governments/{governmentId:int}";

    Task<ApiResponse> UpdateGovernmentAsync(int governmentId, UiUpdateGovernmentDto request, CancellationToken cancellationToken = default);
}

