using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
namespace TransitNova.UI.Areas.CarrierArea.Controllers.Queries;

[Authorize(Roles = Role.Carrier)]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ProfileController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    ICarrierProfileQuery carrierProfileQuery,
    ICarrierProfileCommand carrierProfileCommand)
    : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierProfileQuery.GetCarrierProfileAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        if (response.Data?.Id is Guid profileCarrierId && profileCarrierId != Guid.Empty)
            SetCurrentCarrierId(profileCarrierId);

        return View(response.Data);
    }

    [HttpGet]
    public IActionResult Edit() => View(new CarrierProfileFormViewModel { Id = CurrentCarrierId ?? Guid.Empty });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CarrierProfileFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        if (model.Id != Guid.Empty && model.Id != carrierId)
            return Forbid();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierProfileCommand.UpdateCarrierProfileAsync(model.ToDto(carrierId), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        if (response.Data?.Id is Guid profileCarrierId && profileCarrierId != Guid.Empty)
            SetCurrentCarrierId(profileCarrierId);

        Success("Carrier profile updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult AdditionalInfo() => View(new CarrierAdditionalInfoFormViewModel { Id = CurrentCarrierId ?? Guid.Empty });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdditionalInfo(CarrierAdditionalInfoFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        if (model.Id != Guid.Empty && model.Id != carrierId)
            return Forbid();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierProfileCommand.AddCarrierAdditionalInfoAsync(model.ToDto(carrierId), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Carrier additional info saved successfully.");
        return RedirectToAction(nameof(Index));
    }
}
