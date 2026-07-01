namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;

public interface IRatePickupCarrierCommandService
{
    Task<ApiResponse> RatePickupCarrierAsync(Guid shipmentId, UiRatingCarrierDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

