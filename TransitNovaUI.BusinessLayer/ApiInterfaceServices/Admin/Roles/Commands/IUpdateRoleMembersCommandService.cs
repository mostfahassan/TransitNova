namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface IUpdateRoleMembersCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admin/roles/{roleId:guid}/members";

    Task<ApiResponse> UpdateRoleMembersAsync(Guid roleId, UiUpdateRoleMembersDto request, CancellationToken cancellationToken = default);
}

