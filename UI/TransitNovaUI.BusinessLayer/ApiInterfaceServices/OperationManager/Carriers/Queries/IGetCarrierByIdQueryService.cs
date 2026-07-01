namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Queries;

public interface IGetCarrierByIdQueryService
{
    Task<ApiResponse<UiCarrierProfileDto>> GetCarrierByIdAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

