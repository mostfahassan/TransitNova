namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;

public interface IRatePickupCarrierCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/shipments/{shipmentId:guid}/rate-pickup-carrier";

    Task<ApiResponse> RatePickupCarrierAsync(Guid shipmentId, UiRatingCarrierDto request, CancellationToken cancellationToken = default);
}

