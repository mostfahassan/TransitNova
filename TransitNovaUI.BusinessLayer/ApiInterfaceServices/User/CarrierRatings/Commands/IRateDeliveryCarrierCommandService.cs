namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.CarrierRatings.Commands;

public interface IRateDeliveryCarrierCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/shipments/{shipmentId:guid}/rate-delivery-carrier";

    Task<ApiResponse> RateDeliveryCarrierAsync(Guid shipmentId, UiRatingCarrierDto request, CancellationToken cancellationToken = default);
}

