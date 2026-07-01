namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

public interface ICreateGovernmentCommandService
{
    Task<ApiResponse<UiGovernmentDto>> CreateGovernmentAsync(UiCreateGovernmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

