namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface IDeleteShipmentCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/shipments/{shipmentId:guid}";

    Task<ApiResponse> DeleteShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}

