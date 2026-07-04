using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Location;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class GovernmentsController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminGovernmentQuery adminGovernmentQuery,
    IAdminGovernmentCommand adminGovernmentCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminGovernmentQuery.GetGovernmentsAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{governmentId:int}")]
    public async Task<IActionResult> Details(int governmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminGovernmentQuery.GetGovernmentByIdAsync(governmentId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public IActionResult Create() => View(new GovernmentFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GovernmentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminGovernmentCommand.CreateGovernmentAsync(model.ToCreateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Government created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{governmentId:int}")]
    public IActionResult Edit(int governmentId) => View(new GovernmentFormViewModel());

    [HttpPost("{governmentId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int governmentId, GovernmentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminGovernmentCommand.UpdateGovernmentAsync(governmentId, model.ToUpdateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Government updated successfully.");
        return RedirectToAction(nameof(Details), new { governmentId });
    }

    [HttpPost("{governmentId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int governmentId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminGovernmentCommand.DeleteGovernmentAsync(governmentId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Government deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
