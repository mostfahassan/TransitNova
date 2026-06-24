namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface IUpdateRoleCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admin/roles/{roleId:guid}";

    Task<ApiResponse> UpdateRoleAsync(Guid roleId, UiRoleNameDto request, CancellationToken cancellationToken = default);
}

