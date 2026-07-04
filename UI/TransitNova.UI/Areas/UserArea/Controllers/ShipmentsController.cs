using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Shipments.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ShipmentsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IUserShipmentsQuery userShipmentsQuery,
    IUserShipmentsCommand userShipmentsCommand)
    : AppControllerBase
{
    [HttpGet]
    public IActionResult Create() => View(new CreateShipmentViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateShipmentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var senderId = CurrentUserId;
        if (senderId is null)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.CreateShipmentAsync(model.ToDto(senderId.Value), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Shipment created successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId = response.Data.Id });
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Details(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsQuery.GetUserShipmentByIdAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public IActionResult Edit(Guid shipmentId) => View(new UpdateShipmentViewModel());

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid shipmentId, UpdateShipmentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.UpdateShipmentAsync(shipmentId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Shipment updated successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpGet]
    public IActionResult Track() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TrackSubmit(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            ModelState.AddModelError(nameof(trackingNumber), "Tracking number is required.");
            return View("Track");
        }

        return RedirectToAction(nameof(TrackResult), new { trackingNumber });
    }

    [HttpGet("{trackingNumber}")]
    public async Task<IActionResult> TrackResult(string trackingNumber, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsQuery.TrackShipmentAsync(trackingNumber, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public IActionResult Issue(Guid shipmentId) => View(new ReasonViewModel());

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Issue(Guid shipmentId, ReasonViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.IssueShipmentAsync(shipmentId, model.ToIssueDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Shipment issue submitted successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.CancelShipmentAsync(shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Shipment cancelled successfully.");

        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => userShipmentsCommand.DeleteShipmentAsync(shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            ApiError(response);
            return RedirectToAction(nameof(Details), new { shipmentId });
        }

        Success("Shipment deleted successfully.");
        return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
    }
}
