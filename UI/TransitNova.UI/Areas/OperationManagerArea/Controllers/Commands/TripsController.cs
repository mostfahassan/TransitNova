using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.OperationManager.Segregations.Commands;

namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Commands;

[Authorize(Roles = Role.OperationManager)]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class TripsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IOperationManagerTripsCommand operationManagerTripsCommand)
    : AppControllerBase
{
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
