namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface IDeleteRoleCommandService
{
    Task<ApiResponse> DeleteRoleAsync(Guid roleId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

