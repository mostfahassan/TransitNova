using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Common;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiContracts;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Segregation;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Warehouses.Queries;
using TransitNovaUI.BusinessLayer.DTOs.Warehouse;

namespace TransitNova.UI.Areas.CarrierArea.Controllers.Queries;

[Authorize(Roles = Role.Carrier)]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ProfileController(
    IBackendApiInvoker apiInvoker,
    IIdempotencyKeyFactory idempotencyKeyFactory,
    ICarrierProfileQuery carrierProfileQuery,
    ICarrierProfileCommand carrierProfileCommand,
    IGetSharedWarehousesQueryService warehousesQuery)
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
    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        if (ResolvedCarrierId is not Guid carrierId)
            return Challenge();

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierProfileQuery.GetCarrierProfileAsync(carrierId, token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
            return HandleGetFailure(response);

        return response.Data is null
            ? RedirectToAction("NotFound", "Errors", new { area = "AccountArea" })
            : View(PrefillViewModelFactory.CarrierProfile(response.Data));
    }

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
    public async Task<IActionResult> AdditionalInfo(CancellationToken cancellationToken)
    {
        if (CurrentUserId is null)
            return Challenge();

        var model = new CarrierAdditionalInfoFormViewModel { Id = CurrentCarrierId ?? CurrentUserId.Value };
        await PopulateWarehousesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdditionalInfo(CarrierAdditionalInfoFormViewModel model, CancellationToken cancellationToken)
    {
        if (CurrentUserId is null)
            return Challenge();

        var carrierIdForContractCompatibility = CurrentCarrierId ?? CurrentUserId.Value;

        if (!ModelState.IsValid)
        {
            if (IsAjaxRequest())
                return AjaxValidationFailure();

            await PopulateWarehousesAsync(model, cancellationToken);
            return View(model);
        }

        var response = await apiInvoker.ExecuteAsync((token, ct) => carrierProfileCommand.AddCarrierAdditionalInfoAsync(model.ToDto(carrierIdForContractCompatibility), token!, idempotencyKeyFactory.Create(), ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            if (IsAjaxRequest())
                return AjaxApiFailure(response);

            await PopulateWarehousesAsync(model, cancellationToken);
            return View(model);
        }

        Success("Carrier additional info saved successfully.");
        var redirectUrl = Url.Action("Index", "Dashboard", new { area = "CarrierArea" }) ?? "/CarrierArea/Dashboard/Index";

        return IsAjaxRequest()
            ? Json(new { isSuccess = true, message = "Carrier profile completed successfully.", redirectUrl })
            : RedirectToAction("Index", "Dashboard", new { area = "CarrierArea" });
    }

    private async Task PopulateWarehousesAsync(CarrierAdditionalInfoFormViewModel model, CancellationToken cancellationToken)
    {
        var response = await apiInvoker.ExecuteAsync((token, ct) => warehousesQuery.GetWarehousesAsync(token!, ct), cancellationToken: cancellationToken);
        if (response.IsFailure)
        {
            Error(ApiMessage(response));
            model.Warehouses = [];
            return;
        }

        model.Warehouses = ToWarehouseOptions(response.Data);
    }

    private bool IsAjaxRequest() =>
        string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
        || Request.Headers.Accept.Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);

    private IActionResult AjaxValidationFailure() =>
        UnprocessableEntity(new { isSuccess = false, message = "Please review the highlighted fields.", errors = ModelStateErrors() });

    private IActionResult AjaxApiFailure(ApiResponse response)
    {
        var errors = ModelStateErrors();
        if (errors.Count == 0)
        {
            var messages = response.Errors.Count > 0
                ? response.Errors.Select(error => error.Message).Where(message => !string.IsNullOrWhiteSpace(message)).ToArray()
                : [ApiMessage(response)];

            errors[string.Empty] = messages.Length == 0 ? [ApiMessage(response)] : messages;
        }

        return StatusCode(response.StatusCode >= StatusCodes.Status400BadRequest ? response.StatusCode : StatusCodes.Status400BadRequest,
            new { isSuccess = false, message = ApiMessage(response), errors });
    }

    private Dictionary<string, string[]> ModelStateErrors() =>
        ModelState
            .Where(item => item.Value?.ValidationState == ModelValidationState.Invalid)
            .ToDictionary(
                item => item.Key,
                item => item.Value?.Errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage).ToArray() ?? []);

    private static string ApiMessage(ApiResponse response) =>
        response.Error?.Message
        ?? response.Errors.FirstOrDefault()?.Message
        ?? response.Message
        ?? "The request could not be completed.";

    private static IReadOnlyCollection<WarehouseOptionViewModel> ToWarehouseOptions(IEnumerable<UiWarehouseDto>? warehouses) =>
        warehouses?.Select(warehouse => new WarehouseOptionViewModel(warehouse.Id, warehouse.Name)).ToArray() ?? [];
}