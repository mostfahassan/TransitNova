using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Segregations.Query;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[Authorize(Roles = Role.WarehouseManager)]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class CarriersController(
    IBackendApiInvoker apiInvoker,
    IWarehouseManagerCarriersQuery warehouseManagerCarriersQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CarrierFilterViewModel filter, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerCarriersQuery.GetWarehouseManagerCarriersAsync(warehouseId, filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}")]
    public async Task<IActionResult> Details(Guid carrierId, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerCarriersQuery.GetWarehouseManagerCarrierByIdAsync(warehouseId, carrierId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
