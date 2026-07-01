namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Queries;

public interface IGetUserShipmentByIdQueryService
{
    Task<ApiResponse<UiRetrieveShipmentDto>> GetUserShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
}

