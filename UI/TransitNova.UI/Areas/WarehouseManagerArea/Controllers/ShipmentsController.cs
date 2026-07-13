using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Shipments.Segregations.Query;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[Authorize(Roles = Role.WarehouseManager)]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ShipmentsController(
    IBackendApiInvoker apiInvoker,
    IWarehouseManagerShipmentsQuery warehouseManagerShipmentsQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(UiShipmentFilterDto filter, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerShipmentsQuery.GetWarehouseManagerShipmentsAsync(warehouseId, filter, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Details(Guid shipmentId, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerShipmentsQuery.GetWarehouseManagerShipmentByIdAsync(warehouseId, shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
