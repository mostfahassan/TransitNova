using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Operations;

[Authorize(Roles = Role.OperationManager)]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ShipmentsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IOperationManagerShipmentsQuery operationManagerShipmentsQuery,
    IOperationManagerShipmentsCommand operationManagerShipmentsCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] UiShipmentFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.FilterShipmentsAsync(filter, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public async Task<IActionResult> Assigned([FromQuery] UiShipmentFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.GetAssignedShipmentsAsync(filter, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public async Task<IActionResult> ReviewQueue([FromQuery] UiShipmentFilterDto filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.GetShipmentReviewQueueAsync(filter, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Details(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.GetShipmentByIdAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Histories(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.GetShipmentHistoriesAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> Review(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.ReviewShipmentAsync(shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsCommand.ApproveShipmentAsync(shipmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Shipment approved successfully.");

        return RedirectToAction(nameof(Details), new { shipmentId });
    }

    [HttpGet("{shipmentId:guid}")]
    public IActionResult Reject(Guid shipmentId) => View(new ReasonViewModel());

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid shipmentId, ReasonViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsCommand.RejectShipmentAsync(shipmentId, model.ToRejectDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Shipment rejected successfully.");
        return RedirectToAction(nameof(Details), new { shipmentId });
    }
}
