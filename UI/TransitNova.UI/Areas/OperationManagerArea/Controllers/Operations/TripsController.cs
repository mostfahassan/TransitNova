using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Segregations.Commands;

namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Operations;

[Authorize(Roles = Role.OperationManager)]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class TripsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IOperationManagerTripsCommand operationManagerTripsCommand,
    IOperationManagerTripsQuery operationManagerTripsQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(TripFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerTripsQuery.GetOperationManagerTripsAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);
        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{tripId:guid}")]
    public async Task<IActionResult> Details(Guid tripId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerTripsQuery.GetOperationManagerTripByIdAsync(tripId, token!, ct), cancellationToken: cancellationToken);
        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpPost("{carrierId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartPickup(Guid carrierId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerTripsCommand.StartPickupTripAsync(carrierId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Pickup trip started successfully.");

        return RedirectToAction("Details", "Carriers", new { area = "OperationManagerArea", carrierId });
    }

    [HttpPost("{carrierId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartDelivery(Guid carrierId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerTripsCommand.StartDeliveryTripAsync(carrierId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Delivery trip started successfully.");

        return RedirectToAction("Details", "Carriers", new { area = "OperationManagerArea", carrierId });
    }
}