namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface ICreateShipmentCommandService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> CreateShipmentAsync(UiCreateShipmentDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

