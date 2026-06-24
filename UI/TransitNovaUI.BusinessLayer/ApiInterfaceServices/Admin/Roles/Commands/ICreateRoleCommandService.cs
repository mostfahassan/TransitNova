namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

public interface ICreateRoleCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admin/roles";

    Task<ApiResponse> CreateRoleAsync(UiRoleNameDto request, CancellationToken cancellationToken = default);
}

