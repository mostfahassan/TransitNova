using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Segregation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Vehicles;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class VehiclesController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminVehiclesQuery adminVehiclesQuery,
    IAdminVehiclesCommand adminVehiclesCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminVehiclesQuery.GetVehiclesAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public async Task<IActionResult> Active(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminVehiclesQuery.GetActiveVehiclesAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{vehicleId:guid}")]
    public async Task<IActionResult> Details(Guid vehicleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminVehiclesQuery.GetVehicleByIdAsync(vehicleId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{plateNumber}")]
    public async Task<IActionResult> ByPlate(string plateNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(plateNumber))
            return RedirectToAction(nameof(Index));

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminVehiclesQuery.GetVehicleByPlateNumberAsync(plateNumber, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public IActionResult Create() => View(new VehicleFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminVehiclesCommand.CreateVehicleAsync(model.ToDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Vehicle created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{vehicleId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid vehicleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminVehiclesCommand.DeleteVehicleAsync(vehicleId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Vehicle deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
