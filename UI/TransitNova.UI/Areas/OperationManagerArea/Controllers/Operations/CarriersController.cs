using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Carriers.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Shipments.Segregation;

namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Operations;

[Authorize(Roles = Role.OperationManager)]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class CarriersController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IOperationManagerCarriersQuery operationManagerCarriersQuery,
    IOperationManagerCarriersCommand operationManagerCarriersCommand,
    IOperationManagerShipmentsQuery operationManagerShipmentsQuery)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CarrierFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.FilterCarriersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public async Task<IActionResult> Dispatch(CarrierFilterViewModel filter, CancellationToken cancellationToken)
    {
        filter.Status ??= CarrierStatus.Available;
        filter.AvailableFrom ??= DateTime.UtcNow;
        filter.PageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;

        var carriersResponse = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.FilterCarriersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);
        if (carriersResponse.IsFailure)
            return HandleGetFailure(carriersResponse);

        var shipmentFilter = new ShipmentFilterViewModel
        {
            Status = [ShipmentStatuses.Approved, ShipmentStatuses.InWarehouse],
            PageNumber = 1,
            PageSize = 50
        };

        var shipmentsResponse = await apiInvoker.ExecuteAsync((token, ct) => operationManagerShipmentsQuery.FilterShipmentsAsync(shipmentFilter.ToDto(), token!, ct), cancellationToken: cancellationToken);
        if (shipmentsResponse.IsFailure)
            return HandleGetFailure(shipmentsResponse);

        return View(new OpsDispatchPageViewModel
        {
            Carriers = carriersResponse.Data,
            Shipments = shipmentsResponse.Data
        });
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
    public async Task<IActionResult> AssignPickup(Guid shipmentId, string? city, int? cityId, CancellationToken cancellationToken)
    {
        var model = await BuildAssignmentPageAsync("Pickup", shipmentId, city, cityId, cancellationToken);
        return View(model);
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPickup(Guid shipmentId, OpsAssignCarrierPageViewModel model, CancellationToken cancellationToken)
    {
        if (model.CarrierId == Guid.Empty)
            ModelState.AddModelError(nameof(model.CarrierId), "Select an available pickup carrier.");

        if (!ModelState.IsValid)
            return View(await BuildAssignmentPageAsync("Pickup", shipmentId, model.City, model.CityId, cancellationToken, model.CarrierId, model.ScheduledAt));

        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersCommand.AssignPickupCarrierAsync(shipmentId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(await BuildAssignmentPageAsync("Pickup", shipmentId, model.City, model.CityId, cancellationToken, model.CarrierId, model.ScheduledAt));
        }

        Success("Pickup carrier assigned successfully.");
        return RedirectToAction("Details", "Shipments", new { area = "OperationManagerArea", shipmentId });
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> AssignDelivery(Guid shipmentId, string? city, int? cityId, CancellationToken cancellationToken)
    {
        var model = await BuildAssignmentPageAsync("Delivery", shipmentId, city, cityId, cancellationToken);
        return View(model);
    }

    [HttpPost("{shipmentId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignDelivery(Guid shipmentId, OpsAssignCarrierPageViewModel model, CancellationToken cancellationToken)
    {
        if (model.CarrierId == Guid.Empty)
            ModelState.AddModelError(nameof(model.CarrierId), "Select an available delivery carrier.");

        if (!ModelState.IsValid)
            return View(await BuildAssignmentPageAsync("Delivery", shipmentId, model.City, model.CityId, cancellationToken, model.CarrierId, model.ScheduledAt));

        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersCommand.AssignDeliveryCarrierAsync(shipmentId, model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(await BuildAssignmentPageAsync("Delivery", shipmentId, model.City, model.CityId, cancellationToken, model.CarrierId, model.ScheduledAt));
        }

        Success("Delivery carrier assigned successfully.");
        return RedirectToAction("Details", "Shipments", new { area = "OperationManagerArea", shipmentId });
    }

    private async Task<OpsAssignCarrierPageViewModel> BuildAssignmentPageAsync(string assignmentType, Guid shipmentId, string? city, int? cityId, CancellationToken cancellationToken, Guid selectedCarrierId = default, DateTime? scheduledAt = null)
    {
        var filter = new CarrierFilterViewModel
        {
            Status = CarrierStatus.Available,
            City = string.IsNullOrWhiteSpace(city) || city == "-" ? null : city,
            CityId = cityId,
            AvailableFrom = DateTime.UtcNow,
            PageNumber = 1,
            PageSize = 50
        };

        var response = await apiInvoker.ExecuteAsync((token, ct) => operationManagerCarriersQuery.FilterCarriersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);

        return new OpsAssignCarrierPageViewModel
        {
            ShipmentId = shipmentId,
            AssignmentType = assignmentType,
            CarrierId = selectedCarrierId,
            City = city,
            CityId = cityId,
            ScheduledAt = scheduledAt ?? DateTime.UtcNow.AddHours(1),
            Carriers = response.IsSuccess ? response.Data : null
        };
    }
}

