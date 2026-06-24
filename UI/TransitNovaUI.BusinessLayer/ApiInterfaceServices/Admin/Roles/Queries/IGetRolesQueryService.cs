namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRolesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/roles";

    Task<ApiResponse<List<UiRoleSummaryDto>>> GetRolesAsync(CancellationToken cancellationToken = default);
}

