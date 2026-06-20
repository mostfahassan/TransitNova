namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Commands;

public interface ICreateShipmentCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/shipments";

    Task<ApiResponse<UiRetrieveShipmentDto>> CreateShipmentAsync(
        UiCreateShipmentDto request,
        CancellationToken cancellationToken = default);
}
