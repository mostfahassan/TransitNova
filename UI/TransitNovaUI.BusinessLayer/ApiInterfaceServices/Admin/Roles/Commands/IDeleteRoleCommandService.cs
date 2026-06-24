namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface IDeleteRoleCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admin/roles/{roleId:guid}";

    Task<ApiResponse> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
}

