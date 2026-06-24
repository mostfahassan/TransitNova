namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface ICompleteDeliveryCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}shipments/{shipmentId:guid}/complete-delivery";

    Task<ApiResponse> CompleteDeliveryAsync(Guid carrierId, Guid shipmentId, CancellationToken cancellationToken = default);
}

