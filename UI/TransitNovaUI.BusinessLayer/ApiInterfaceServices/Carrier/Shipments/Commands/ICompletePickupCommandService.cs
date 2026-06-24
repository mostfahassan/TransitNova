namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Commands;

public interface ICompletePickupCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/shipments/{shipmentId:guid}/complete-pickup";

    Task<ApiResponse> CompletePickupAsync(Guid carrierId, Guid shipmentId, CancellationToken cancellationToken = default);
}

