using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Carriers.Segregations.Query;
using TransitNovaUI.BusinessLayer.DTOs.Carrier;
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
    public async Task<IActionResult> Index(UiFilterCarrierDto filter, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerCarriersQuery.GetWarehouseManagerCarriersAsync(warehouseId, filter, token!, ct), cancellationToken: cancellationToken);

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
