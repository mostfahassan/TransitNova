using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Queries;

namespace TransitNova.UI.Infrastructure.Mvc.Implementation;

public sealed class WarehouseContextService(
    IUiAuthSessionService authSession,
    IBackendApiInvoker apiInvoker,
    IGetWarehouseManagerDashboardQueryService dashboardQuery)
    : IWarehouseContextService
{
    public async Task<Guid?> GetWarehouseIdAsync(CancellationToken cancellationToken = default)
    {
        var sessionValue = authSession.GetWarehouseId();
        if (Guid.TryParse(sessionValue, out var warehouseId))
            return warehouseId;

        var response = await apiInvoker.ExecuteAsync(
            (token, ct) => dashboardQuery.GetWarehouseManagerDashboardAsync(token!, ct),
            cancellationToken: cancellationToken);

        var discoveredWarehouseId = response.Data?.Manager.WarehouseId;
        if (response.IsSuccess && discoveredWarehouseId is Guid value && value != Guid.Empty)
        {
            authSession.SetWarehouseId(value);
            return value;
        }

        return null;
    }
}
