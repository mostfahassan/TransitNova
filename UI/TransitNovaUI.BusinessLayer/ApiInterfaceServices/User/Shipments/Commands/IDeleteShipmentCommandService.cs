namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface IDeleteShipmentCommandService
{
    Task<ApiResponse> DeleteShipmentAsync(Guid shipmentId, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

