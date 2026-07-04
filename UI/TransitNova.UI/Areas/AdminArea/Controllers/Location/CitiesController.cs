using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Query;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Location;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class CitiesController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminCityQuery adminCityQuery,
    IAdminCityCommand adminCityCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CityFilterViewModel filter, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCityQuery.FilterCitiesAsync(filter.ToDto(), token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{cityId:int}")]
    public async Task<IActionResult> Details(int cityId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCityQuery.GetCityByIdAsync(cityId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public IActionResult Create() => View(new CityFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CityFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCityCommand.CreateCityAsync(model.ToCreateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("City created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{cityId:int}")]
    public IActionResult Edit(int cityId) => View(new CityFormViewModel());

    [HttpPost("{cityId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int cityId, CityFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCityCommand.UpdateCityAsync(cityId, model.ToUpdateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("City updated successfully.");
        return RedirectToAction(nameof(Details), new { cityId });
    }

    [HttpPost("{cityId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int cityId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminCityCommand.DeleteCityAsync(cityId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("City deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
