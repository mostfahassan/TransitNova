namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

public interface ICreateGovernmentCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admin/governments";

    Task<ApiResponse<UiGovernmentDto>> CreateGovernmentAsync(UiCreateGovernmentDto request, CancellationToken cancellationToken = default);
}

