namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface ICreateRoleCommandService
{
    Task<ApiResponse> CreateRoleAsync(UiRoleNameDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

