using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Commands;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Query;

namespace TransitNova.UI.Areas.AdminArea.Controllers.Bundles;

[Authorize(Roles = Role.Admin)]
[Area("AdminArea")]
[Route("[area]/[controller]/[action]")]
public sealed class BundlesController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    IAdminBundlesQuery adminBundlesQuery,
    IAdminBundlesCommand adminBundlesCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminBundlesQuery.GetBundlesAsync(token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet("{bundleId:int}")]
    public async Task<IActionResult> Details(int bundleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminBundlesQuery.GetBundleByIdAsync(bundleId, token!, ct), cancellationToken: cancellationToken);

        return response.IsSuccess ? View(response.Data) : HandleGetFailure(response);
    }

    [HttpGet]
    public IActionResult Create() => View(new BundleFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BundleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminBundlesCommand.CreateBundleAsync(model.ToCreateDto(), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Bundle created successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{bundleId:guid}")]
    public IActionResult Edit(Guid bundleId) => View(new BundleFormViewModel { BundleId = bundleId });

    [HttpPost("{bundleId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid bundleId, BundleFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => adminBundlesCommand.UpdateBundleAsync(model.ToUpdateDto(bundleId), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Bundle updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{bundleId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int bundleId, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => adminBundlesCommand.DeleteBundleAsync(bundleId, token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            ApiError(response);
        else
            Success("Bundle deleted successfully.");

        return RedirectToAction(nameof(Index));
    }
}
