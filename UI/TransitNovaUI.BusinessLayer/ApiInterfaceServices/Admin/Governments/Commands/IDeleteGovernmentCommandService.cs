namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

public interface IDeleteGovernmentCommandService
{
    Task<ApiResponse> DeleteGovernmentAsync(int governmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

