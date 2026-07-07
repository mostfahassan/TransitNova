using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Shipments.Segregation;
namespace TransitNova.UI.Areas.CarrierArea.Controllers.Queries;

[Authorize(Roles = Role.Carrier)]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ShipmentsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    ICarrierShipmentsQuery carrierShipmentsQuery,
    ICarrierShipmentsCommand carrierShipmentsCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CarrierShipmentFilterViewModel filter, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierShipmentsQuery.GetCarrierShipmentsAsync(carrierId, filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Details(Guid shipmentId, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierShipmentsQuery.GetCarrierShipmentByIdAsync(carrierId, shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }


    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PickedUp(Guid shipmentId, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierShipmentsCommand.MarkShipmentPickedUpAsync(carrierId, shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Shipment picked up successfully.");

        return RedirectToAction(nameof(Details), new { shipmentId });
    }
    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompletePickup(Guid shipmentId, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierShipmentsCommand.CompletePickupAsync(carrierId, shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Pickup completed successfully.");

        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteDelivery(Guid shipmentId, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierShipmentsCommand.CompleteDeliveryAsync(carrierId, shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Delivery completed successfully.");

        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpGet]
    public IActionResult Status() => View(new CarrierStatusFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(CarrierStatusFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierShipmentsCommand.UpdateCarrierStatusAsync(carrierId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Carrier status updated successfully.");
        return RedirectToAction(nameof(Index));
    }
}

