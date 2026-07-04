namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.WarehouseManagers.Queries;

public interface IGetWarehouseManagersQueryService
{
    Task<ApiResponse<UiPagedResult<UiWarehouseManagerListDto>>> GetWarehouseManagersAsync(UiWarehouseManagerFilterDto filter, string bearerToken, CancellationToken cancellationToken = default);
}
