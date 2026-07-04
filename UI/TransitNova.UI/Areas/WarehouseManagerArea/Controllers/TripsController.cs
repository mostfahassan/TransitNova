using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.WarehouseManager.Trips.Segregations.Query;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[Authorize(Roles = Role.WarehouseManager)]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class TripsController(
    IBackendApiInvoker apiInvoker,
    IWarehouseManagerTripsQuery warehouseManagerTripsQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(TripFilterViewModel filter, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        filter.WarehouseId = warehouseId;
        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerTripsQuery.GetWarehouseManagerTripsAsync(warehouseId, filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{tripId:guid}")]
    public async Task<IActionResult> Details(Guid tripId, CancellationToken cancellationToken)
    {
        if (CurrentWarehouseId is not Guid warehouseId)
            return RedirectToAction("Index", "Dashboard", new { area = "WarehouseManagerArea" });

        var response = await apiInvoker.ExecuteAsync((token, ct) => warehouseManagerTripsQuery.GetWarehouseManagerTripByIdAsync(warehouseId, tripId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }
}
