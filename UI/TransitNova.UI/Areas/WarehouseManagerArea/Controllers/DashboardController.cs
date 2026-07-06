using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Query;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[Authorize(Roles = Role.WarehouseManager)]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController(
    IBackendApiInvoker apiInvoker,
    IWarehouseManagerDashboardQuery warehouseManagerDashboardQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerDashboardQuery.GetWarehouseManagerDashboardAsync(token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        if (response.Data?.Manager.WarehouseId is Guid warehouseId && warehouseId != Guid.Empty)
            SetCurrentWarehouseId(warehouseId);

        return View(response.Data);
    }
}
