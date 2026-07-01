namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IGetAssignedShipmentsQueryService
{
    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetAssignedShipmentsAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

