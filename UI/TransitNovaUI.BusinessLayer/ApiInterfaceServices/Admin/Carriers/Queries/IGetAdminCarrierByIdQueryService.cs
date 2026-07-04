namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Queries;

public interface IGetAdminCarrierByIdQueryService
{
    Task<ApiResponse<UiCarrierProfileDto>> GetAdminCarrierByIdAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

