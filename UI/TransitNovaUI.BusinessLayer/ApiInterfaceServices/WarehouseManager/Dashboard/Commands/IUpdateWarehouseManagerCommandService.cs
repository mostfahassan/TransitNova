namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Commands;

public interface IUpdateWarehouseManagerCommandService
{
    Task<ApiResponse> UpdateWarehouseManagerAsync(UiUpdateWarehouseManagerProfileDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}
