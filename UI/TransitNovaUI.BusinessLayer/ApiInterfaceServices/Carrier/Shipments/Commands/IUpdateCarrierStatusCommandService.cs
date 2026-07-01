namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface IUpdateCarrierStatusCommandService
{
    Task<ApiResponse> UpdateCarrierStatusAsync(Guid carrierId, UiChangeCarrierStatusDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

