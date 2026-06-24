namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRoleByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/roles/{roleId:guid}";

    Task<ApiResponse<UiRoleSummaryDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}

