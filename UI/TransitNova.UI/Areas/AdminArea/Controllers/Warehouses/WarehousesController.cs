using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Warehouses.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Warehouses;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class WarehousesController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminWarehousesQuery adminWarehousesQuery,
    IAdminWarehousesCommand adminWarehousesCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehousesQuery.GetWarehousesAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{warehouseId:guid}")]
    public async Task<IActionResult> Details(Guid warehouseId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehousesQuery.GetWarehouseByIdAsync(warehouseId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public IActionResult Create() => View(new WarehouseFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehousesCommand.CreateWarehouseAsync(model.ToCreateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Warehouse created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{warehouseId:guid}")]
    public IActionResult Edit(Guid warehouseId) => View(new WarehouseFormViewModel());

    [HttpPost("{warehouseId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid warehouseId, WarehouseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehousesCommand.UpdateWarehouseAsync(warehouseId, model.ToUpdateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Warehouse updated successfully.");
        return RedirectToAction(nameof(Details), new { warehouseId });
    }

    [HttpPost("{warehouseId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid warehouseId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminWarehousesCommand.DeleteWarehouseAsync(warehouseId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Warehouse deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
