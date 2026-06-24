namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

public interface IDeleteGovernmentCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admin/governments/{governmentId:int}";

    Task<ApiResponse> DeleteGovernmentAsync(int governmentId, CancellationToken cancellationToken = default);
}

