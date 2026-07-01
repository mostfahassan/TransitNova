namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

public interface IUpdateGovernmentCommandService
{
    Task<ApiResponse> UpdateGovernmentAsync(int governmentId, UiUpdateGovernmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

