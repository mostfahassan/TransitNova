namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Shipment.Query
{
    public interface IGetAdminShipmentsQueryService
    {
        Task<ApiResponse<UiPagedResult<UiRetrieveShipmentDto>>> GetAdminShipmentsAsync(UiShipmentFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
    }
}
