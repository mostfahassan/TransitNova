using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Query;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[Authorize(Roles = Role.WarehouseManager)]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ProfileController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IWarehouseManagerDashboardQuery warehouseManagerDashboardQuery,
    IWarehouseManagerDashboardCommand warehouseManagerDashboardCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerDashboardQuery.GetWarehouseManagerDashboardAsync(token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        if (response.Data?.Manager.WarehouseId is Guid warehouseId && warehouseId != Guid.Empty)
            SetCurrentWarehouseId(warehouseId);

        return response.Data is null
            ? RedirectToAction("NotFound", "Errors", new { area = "AccountArea" })
            : View(PrefillViewModelFactory.WarehouseManagerProfile(response.Data));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WarehouseManagerProfileFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerDashboardCommand.UpdateWarehouseManagerAsync(model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        if (model.WarehouseId is Guid warehouseId && warehouseId != Guid.Empty)
            SetCurrentWarehouseId(warehouseId);

        Success("Warehouse manager profile updated successfully.");
        return RedirectToAction(nameof(Edit));
    }
}