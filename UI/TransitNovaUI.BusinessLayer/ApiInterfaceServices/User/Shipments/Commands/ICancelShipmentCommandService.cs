namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface ICancelShipmentCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/shipments/{shipmentId:guid}";

    Task<ApiResponse> CancelShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}

