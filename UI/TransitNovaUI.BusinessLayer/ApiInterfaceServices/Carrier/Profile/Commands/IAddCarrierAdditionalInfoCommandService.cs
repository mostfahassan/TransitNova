namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Commands;

public interface IAddCarrierAdditionalInfoCommandService
{
    Task<ApiResponse> AddCarrierAdditionalInfoAsync(UiAdditionalInfoDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

