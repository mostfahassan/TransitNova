namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

public interface IGetRoleMembersQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/roles/{roleId:guid}/members";

    Task<ApiResponse<UiRoleMembersDto>> GetRoleMembersAsync(Guid roleId, CancellationToken cancellationToken = default);
}

