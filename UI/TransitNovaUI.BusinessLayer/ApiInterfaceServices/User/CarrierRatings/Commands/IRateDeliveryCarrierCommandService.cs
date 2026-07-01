namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;

public interface IRateDeliveryCarrierCommandService
{
    Task<ApiResponse> RateDeliveryCarrierAsync(Guid shipmentId, UiRatingCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

