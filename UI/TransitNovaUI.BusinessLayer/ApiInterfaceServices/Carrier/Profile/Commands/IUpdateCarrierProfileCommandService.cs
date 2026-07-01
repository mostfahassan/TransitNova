namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;

public interface IUpdateCarrierProfileCommandService
{
    Task<ApiResponse<UiCarrierProfileDto>> UpdateCarrierProfileAsync(UiUpdateCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

