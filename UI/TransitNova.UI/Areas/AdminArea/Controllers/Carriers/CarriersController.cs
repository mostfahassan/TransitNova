using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Segregations.Query;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Carriers;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class CarriersController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminCarriersQuery adminCarriersQuery,
    IAdminCarriersCommand adminCarriersCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CarrierFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCarriersQuery.GetAdminCarriersAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}")]
    public async Task<IActionResult> Details(Guid carrierId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCarriersQuery.GetAdminCarrierByIdAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}")]
    public async Task<IActionResult> Shipments(Guid carrierId, CarrierShipmentFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCarriersQuery.GetAdminCarrierShipmentsAsync(carrierId, filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{carrierId:guid}/{shipmentId:guid}")]
    public async Task<IActionResult> ShipmentDetails(Guid carrierId, Guid shipmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCarriersQuery.GetAdminCarrierShipmentByIdAsync(carrierId, shipmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpPost("{carrierId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid carrierId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCarriersCommand.DeleteCarrierAsync(carrierId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Carrier deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
