namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Queries;

public interface IGetGovernmentsQueryService
{
    Task<ApiResponse<List<UiGovernmentDto>>> GetGovernmentsAsync(string bearerToken, CancellationToken cancellationToken = default);
}

