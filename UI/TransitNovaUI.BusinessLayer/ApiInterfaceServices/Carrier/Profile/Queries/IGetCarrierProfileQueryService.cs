namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Queries;

public interface IGetCarrierProfileQueryService
{
    Task<ApiResponse<UiCarrierProfileDto>> GetCarrierProfileAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

