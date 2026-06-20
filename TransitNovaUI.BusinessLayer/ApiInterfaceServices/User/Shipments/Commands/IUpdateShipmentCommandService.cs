namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface IUpdateShipmentCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/shipments/{shipmentId:guid}";

    Task<ApiResponse> UpdateShipmentAsync(Guid shipmentId, UiUpdateShipmentDto request, CancellationToken cancellationToken = default);
}

