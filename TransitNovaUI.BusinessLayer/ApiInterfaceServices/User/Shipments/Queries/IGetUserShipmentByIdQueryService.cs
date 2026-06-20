namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;

public interface IGetUserShipmentByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/user/shipments/{shipmentId:guid}";

    Task<ApiResponse<UiRetrieveShipmentDto>> GetUserShipmentByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
}

