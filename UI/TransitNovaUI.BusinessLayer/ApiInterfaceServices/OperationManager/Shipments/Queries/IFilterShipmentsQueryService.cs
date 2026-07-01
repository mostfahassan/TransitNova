namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Queries;

public interface IFilterShipmentsQueryService
{
    Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> FilterShipmentsAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}

