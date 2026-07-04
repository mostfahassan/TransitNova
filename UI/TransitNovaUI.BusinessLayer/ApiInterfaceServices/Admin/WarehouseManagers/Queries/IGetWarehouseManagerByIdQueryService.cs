namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.WarehouseManagers.Queries;

public interface IGetWarehouseManagerByIdQueryService
{
    Task<ApiResponse<UiWarehouseManagerDetailsDto>> GetWarehouseManagerByIdAsync(Guid id, string bearerToken, CancellationToken cancellationToken = default);
}
