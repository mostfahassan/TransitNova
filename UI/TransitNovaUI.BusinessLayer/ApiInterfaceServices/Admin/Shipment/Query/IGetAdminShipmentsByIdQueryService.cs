namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Shipment.Query
{
    public interface IGetAdminShipmentsByIdQueryService
    {
        Task<ApiResponse<UiRetrieveShipmentDto>> GetAdminShipmentByIdAsync(Guid shipmentId, string bearerToken, CancellationToken cancellationToken = default);
    }
}
