namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface IUpdateShipmentCommandService
{
    Task<ApiResponse> UpdateShipmentAsync(Guid shipmentId, UiUpdateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

