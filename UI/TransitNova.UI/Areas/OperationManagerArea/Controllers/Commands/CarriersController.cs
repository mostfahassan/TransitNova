using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;

namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Commands;

[Authorize(Roles = Role.OperationManager)]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class CarriersController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IOperationManagerCarriersQuery operationManagerCarriersQuery,
    IOperationManagerCarriersCommand operationManagerCarriersCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CarrierFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.FilterCarriersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}")]
    public async Task<IActionResult> Details(Guid carrierId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.GetCarrierByIdAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}")]
    public async Task<IActionResult> Shipments(Guid carrierId, CarrierShipmentFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.GetCarrierShipmentsAsync(carrierId, filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}/{shipmentId:guid}")]
    public async Task<IActionResult> ShipmentDetails(Guid carrierId, Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.GetCarrierShipmentByIdAsync(carrierId, shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public IActionResult AssignPickup(Guid shipmentId) => View(new AssignCarrierViewModel());

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPickup(Guid shipmentId, AssignCarrierViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersCommand.AssignPickupCarrierAsync(shipmentId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Pickup carrier assigned successfully.");
        return RedirectToAction("Details", "Shipments", new { area = "OperationManagerArea", shipmentId });
    }

    [HttpGet("{shipmentId:guid}")]
    public IActionResult AssignDelivery(Guid shipmentId) => View(new AssignCarrierViewModel());

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignDelivery(Guid shipmentId, AssignCarrierViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersCommand.AssignDeliveryCarrierAsync(shipmentId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Delivery carrier assigned successfully.");
        return RedirectToAction("Details", "Shipments", new { area = "OperationManagerArea", shipmentId });
    }
}
