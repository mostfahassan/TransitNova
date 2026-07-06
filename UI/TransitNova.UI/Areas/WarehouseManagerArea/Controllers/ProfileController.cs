using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
 
 
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Dashboard.Segregations.Commands;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[Authorize(Roles = Role.WarehouseManager)]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ProfileController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IWarehouseManagerDashboardCommand warehouseManagerDashboardCommand)
    : AppControllerBase
{
    [HttpGet]
    public IActionResult Edit() => View(new WarehouseManagerProfileFormViewModel());

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

        Success("Warehouse manager profile updated successfully.");
        return RedirectToAction(nameof(Edit));
    }
}
