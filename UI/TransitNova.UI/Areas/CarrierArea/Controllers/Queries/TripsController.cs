using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Trips.Carriers.Segregations.Query;

namespace TransitNova.UI.Areas.CarrierArea.Controllers.Queries;

[Authorize(Roles = Role.Carrier)]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class TripsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    ICarrierTripsQuery carrierTripsQuery,
    ICarrierTripsCommand carrierTripsCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierTripsQuery.GetCarrierTripsAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{tripId:guid}")]
    public async Task<IActionResult> Details(Guid tripId, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierTripsQuery.GetCarrierTripByIdAsync(carrierId, tripId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpPost("{tripId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid tripId, CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierTripsCommand.CompleteCarrierTripAsync(carrierId, tripId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Trip completed successfully.");

        return RedirectToAction(nameof(Details), new { tripId });
    }
}