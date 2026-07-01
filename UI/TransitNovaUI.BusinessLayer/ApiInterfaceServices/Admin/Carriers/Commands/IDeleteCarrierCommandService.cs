namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Commands;

public interface IDeleteCarrierCommandService
{
    Task<ApiResponse> DeleteCarrierAsync(Guid id, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

